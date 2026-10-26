using System;
using System.Text;

namespace mrzha
{
    /// <summary>
    /// Amplop rahasia bertanda tangan yang disimpan di bagian tersembunyi QR.
    ///
    /// Amplop  : [VERSI 0x01][keyId][rahasia UTF-8 ...][tanda tangan 64 byte]
    /// Ditandatangani (ECDSA P-256 / SHA-256, r||s):
    ///           [VERSI][keyId][panjang main 2 byte big-endian][main UTF-8][rahasia UTF-8]
    ///
    /// Main ikut ditandatangani supaya rahasia dari QR asli tidak bisa ditempel ke QR lain.
    /// Rahasia tetap bisa DIBACA oleh siapa pun yang mengerti formatnya; yang dijamin adalah
    /// rahasia tidak bisa DIBUAT/DIUBAH tanpa kunci privat. Salinan QR asli dideteksi di server
    /// (counter scan + lokasi), bukan di sini.
    /// </summary>
    public static class TandaRahasia
    {
        public const byte VERSI = 0x01;
        public const int PANJANG_TANDA = 64;
        public const int MAX_RAHASIA_BYTES = zvr.QrCode.Internal.Encoder.MAX_RAHASIA_BYTES - 2 - PANJANG_TANDA;

        public static byte[] Bungkus(KunciQr kunci, string main, string rahasia)
        {
            byte[] rahasiaBytes = Encoding.UTF8.GetBytes(rahasia ?? "");
            if (rahasiaBytes.Length > MAX_RAHASIA_BYTES)
                throw new ArgumentException("Rahasia terlalu panjang: " + rahasiaBytes.Length + " byte (maks " + MAX_RAHASIA_BYTES + ")");

            byte[] tanda = kunci.Tandatangani(PesanDitandatangani(kunci.KeyId, main, rahasiaBytes));

            byte[] amplop = new byte[2 + rahasiaBytes.Length + PANJANG_TANDA];
            amplop[0] = VERSI;
            amplop[1] = kunci.KeyId;
            Buffer.BlockCopy(rahasiaBytes, 0, amplop, 2, rahasiaBytes.Length);
            Buffer.BlockCopy(tanda, 0, amplop, 2 + rahasiaBytes.Length, PANJANG_TANDA);
            return amplop;
        }

        /// <summary>Ambil keyId dari amplop (untuk memilih kunci publik), -1 bila bukan amplop.</summary>
        public static int KeyIdDari(byte[] amplop)
        {
            if (amplop == null || amplop.Length < 2 + PANJANG_TANDA || amplop[0] != VERSI)
                return -1;
            return amplop[1];
        }

        /// <summary>
        /// Verifikasi amplop. true = tanda tangan sah untuk main + rahasia ini.
        /// </summary>
        public static bool Buka(KunciQr kunci, string main, byte[] amplop, out string rahasia, out string alasan)
        {
            rahasia = null;
            int keyId = KeyIdDari(amplop);
            if (keyId < 0)
            {
                alasan = "Bukan amplop bertanda tangan (versi/panjang salah)";
                return false;
            }
            if (keyId != kunci.KeyId)
            {
                alasan = "KeyId " + keyId + " tidak cocok dengan kunci " + kunci.KeyId;
                return false;
            }

            int panjangRahasia = amplop.Length - 2 - PANJANG_TANDA;
            byte[] rahasiaBytes = new byte[panjangRahasia];
            byte[] tanda = new byte[PANJANG_TANDA];
            Buffer.BlockCopy(amplop, 2, rahasiaBytes, 0, panjangRahasia);
            Buffer.BlockCopy(amplop, 2 + panjangRahasia, tanda, 0, PANJANG_TANDA);

            if (!kunci.Verifikasi(PesanDitandatangani(kunci.KeyId, main, rahasiaBytes), tanda))
            {
                alasan = "Tanda tangan tidak sah (QR palsu atau data diubah)";
                return false;
            }
            rahasia = Encoding.UTF8.GetString(rahasiaBytes);
            alasan = "OK";
            return true;
        }

        public static byte[] PesanDitandatangani(byte keyId, string main, byte[] rahasiaBytes)
        {
            byte[] mainBytes = Encoding.UTF8.GetBytes(main ?? "");
            if (mainBytes.Length > 0xFFFF)
                throw new ArgumentException("Main terlalu panjang");
            byte[] pesan = new byte[4 + mainBytes.Length + rahasiaBytes.Length];
            pesan[0] = VERSI;
            pesan[1] = keyId;
            pesan[2] = (byte)(mainBytes.Length >> 8);
            pesan[3] = (byte)mainBytes.Length;
            Buffer.BlockCopy(mainBytes, 0, pesan, 4, mainBytes.Length);
            Buffer.BlockCopy(rahasiaBytes, 0, pesan, 4 + mainBytes.Length, rahasiaBytes.Length);
            return pesan;
        }
    }
}
