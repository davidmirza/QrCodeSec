using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using mrzha;
using ZXing;

namespace QrSecDesigner
{
    /// <summary>
    /// Logika pembuat QR qrsec (tanpa UI) - dipakai FormUtama dan bisa diuji terpisah.
    /// Signing aktif  : Buat.CetakAman  -> secret dibungkus tanda tangan ECDSA (APK: "Signing key DITEMUKAN").
    /// Signing mati   : Buat.Cetak      -> hanya main + secret polos     (APK: "Signing key TIDAK AKTIF").
    /// </summary>
    public static class PembuatQr
    {
        public static readonly string[] LEVEL = { "L", "M", "Q", "H" };

        /// <summary>Batas byte UTF-8 secret: 156 bila bertanda tangan, 222 bila tanpa.</summary>
        public static int BatasSecret(bool signing)
        {
            return signing ? TandaRahasia.MAX_RAHASIA_BYTES : zvr.QrCode.Internal.Encoder.MAX_RAHASIA_BYTES;
        }

        public static int JumlahByte(string teks)
        {
            return Encoding.UTF8.GetByteCount(teks ?? "");
        }

        /// <summary>null = valid; selain itu pesan kesalahan untuk pengguna.</summary>
        public static string Validasi(string main, string secret, bool signing, KunciQr kunci)
        {
            if (string.IsNullOrEmpty(main))
                return "Isi QR (main) tidak boleh kosong.";
            int n = JumlahByte(secret);
            if (n > BatasSecret(signing))
                return "Secret terlalu panjang: " + n + " byte (maks " + BatasSecret(signing) + " byte"
                       + (signing ? " saat signing aktif)." : ").");
            if (signing)
            {
                if (kunci == null)
                    return "Signing aktif tetapi file kunci (.key) belum dimuat.\nPilih / buat kunci, atau matikan signing.";
                if (!kunci.PunyaPrivat)
                    return "Kunci yang dimuat hanya kunci publik, tidak bisa menandatangani.";
            }
            else if (!string.IsNullOrEmpty(secret) && secret[0] == '\u0001')
            {
                // byte 0x01 di awal = penanda amplop bertanda tangan; APK akan salah mengira ada signing key
                return "Saat signing dimatikan, secret tidak boleh diawali karakter kontrol U+0001.";
            }
            return null;
        }

        public static Bitmap Buat(string main, string secret, bool signing, KunciQr kunci, string level, int ukuran)
        {
            return signing
                ? mrzha.Buat.CetakAman(main, secret ?? "", kunci, level, ukuran)
                : mrzha.Buat.Cetak(main, secret ?? "", level, ukuran);
        }

        /// <summary>
        /// Baca ulang gambar dengan decoder ZXing (seperti scanner HP) dan cocokkan main, secret,
        /// dan tanda tangan. Laporan berisi baris-baris hasil cek.
        /// </summary>
        public static bool Verifikasi(Bitmap gambar, string main, string secret, bool signing, KunciQr kunci,
                                      out string laporan)
        {
            var sb = new StringBuilder();
            var reader = new BarcodeReader { AutoRotate = false };
            reader.Options.PossibleFormats = new List<BarcodeFormat> { BarcodeFormat.QR_CODE };
            reader.Options.TryHarder = true;
            Result r = reader.Decode(gambar);
            if (r == null)
            {
                laporan = "✖ Gambar tidak terbaca decoder QR.";
                return false;
            }
            bool ok = r.Text == main;
            sb.AppendLine((ok ? "✔" : "✖") + " Scanner biasa membaca: " + Pendek(r.Text));

            byte[] mainBytes, amplop;
            if (!PembacaRaw.Ambil(r.RawBytes, out mainBytes, out amplop))
            {
                laporan = sb.AppendLine("✖ Bagian secret qrsec tidak ditemukan.").ToString();
                return false;
            }

            bool adaTanda = TandaRahasia.KeyIdDari(amplop) >= 0;
            if (signing)
            {
                string isi, alasan;
                bool sah = adaTanda && TandaRahasia.Buka(kunci, main, amplop, out isi, out alasan) && isi == (secret ?? "");
                sb.AppendLine((sah ? "✔" : "✖") + " Signing key: DITEMUKAN (keyId " + TandaRahasia.KeyIdDari(amplop) + ") - "
                              + (sah ? "tanda tangan sah" : "TIDAK sah"));
                sb.AppendLine("  Secret: " + Pendek(secret));
                ok &= sah;
            }
            else
            {
                string isi = Encoding.UTF8.GetString(amplop);
                bool cocok = !adaTanda && isi == (secret ?? "");
                sb.AppendLine((cocok ? "✔" : "✖") + " Signing key: TIDAK AKTIF - secret polos "
                              + (cocok ? "terbaca" : "TIDAK cocok"));
                sb.AppendLine("  Secret: " + Pendek(isi));
                ok &= cocok;
            }
            sb.Append("  Bagian tersembunyi: " + amplop.Length + " byte");
            laporan = sb.ToString();
            return ok;
        }

        /// <summary>Baris siap tempel untuk QrSec.PUBLIC_KEYS di APK.</summary>
        public static string BarisJava(KunciQr kunci)
        {
            return "PUBLIC_KEYS.put(" + kunci.KeyId + ", \"" + kunci.PublicKeyBase64 + "\");";
        }

        private static string Pendek(string s)
        {
            if (s == null) return "-";
            s = s.Replace("\r", "").Replace("\n", "⏎");
            return s.Length > 60 ? s.Substring(0, 60) + "… (" + s.Length + " karakter)" : s;
        }
    }
}
