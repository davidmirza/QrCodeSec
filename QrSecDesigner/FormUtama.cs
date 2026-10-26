using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Windows.Forms;
using mrzha;

namespace QrSecDesigner
{
    /// <summary>
    /// Pembuat QR qrsec: main + secret, dengan opsi signing key (tanda tangan ECDSA) bisa dimatikan.
    /// </summary>
    public partial class FormUtama : Form
    {
        private static readonly Color WARNA_OK = Color.FromArgb(232, 245, 233);
        private static readonly Color WARNA_GAGAL = Color.FromArgb(255, 235, 238);

        private readonly Pengaturan setel = Pengaturan.Muat();
        private KunciQr kunci;
        private Bitmap gambar;

        public FormUtama()
        {
            InitializeComponent();
        }

        private bool Signing
        {
            get { return chkSigning.Checked; }
        }

        private string Level
        {
            get { return PembuatQr.LEVEL[Math.Max(0, cmbEc.SelectedIndex)]; }
        }

        private void FormUtama_Load(object sender, EventArgs e)
        {
            chkSigning.Checked = setel.Signing;
            cmbEc.SelectedIndex = Math.Max(0, Array.IndexOf(PembuatQr.LEVEL, setel.Level));
            cmbUkuran.Text = setel.Ukuran.ToString();
            if (!string.IsNullOrEmpty(setel.FileKunci) && File.Exists(setel.FileKunci))
                MuatKunci(setel.FileKunci, true);
            AturSigning();
            UpdateHitung();
            TandaiBelumDibuat("Isi main + secret, pilih setup, lalu klik Buat QR.\r\nQR hasil akan dibaca ulang otomatis untuk verifikasi.");
        }

        private void FormUtama_FormClosing(object sender, FormClosingEventArgs e)
        {
            setel.Signing = Signing;
            setel.Level = Level;
            int ukuran;
            if (int.TryParse(cmbUkuran.Text, out ukuran)) setel.Ukuran = ukuran;
            setel.Simpan();
            if (kunci != null) kunci.Dispose();
        }

        private void chkSigning_CheckedChanged(object sender, EventArgs e)
        {
            AturSigning();
            Isi_TextChanged(sender, e);
        }

        private void AturSigning()
        {
            bool on = Signing;
            foreach (Control c in new Control[] { lblKunci, txtKunci, btnPilihKunci, lblKeyIdBaru, numKeyId, btnBuatKunci, lblInfoKunci })
                c.Enabled = on;
            btnSalinPublik.Enabled = on && kunci != null;
            UpdateHitung();
        }

        private void Isi_TextChanged(object sender, EventArgs e)
        {
            UpdateHitung();
            if (gambar != null)
                TandaiBelumDibuat("Setup / isi berubah - klik Buat QR lagi.");
        }

        private void UpdateHitung()
        {
            int n = PembuatQr.JumlahByte(txtSecret.Text);
            int batas = PembuatQr.BatasSecret(Signing);
            lblHitung.Text = "Secret: " + n + " / " + batas + " byte" + (Signing ? " (signing aktif)" : " (tanpa signing)");
            lblHitung.ForeColor = n > batas ? Color.Firebrick : SystemColors.ControlText;
        }

        private void TandaiBelumDibuat(string pesan)
        {
            lblVerifikasi.Text = pesan;
            lblVerifikasi.BackColor = Color.WhiteSmoke;
        }

        // ---------------- Buat / simpan ----------------

        private void btnBuat_Click(object sender, EventArgs e)
        {
            int ukuran;
            if (!int.TryParse(cmbUkuran.Text.Trim(), out ukuran) || ukuran < 100 || ukuran > 2000)
            {
                Pesan("Ukuran gambar harus angka 100 - 2000 px.", MessageBoxIcon.Warning);
                return;
            }
            string main = txtMain.Text, secret = txtSecret.Text;
            string salah = PembuatQr.Validasi(main, secret, Signing, kunci);
            if (salah != null)
            {
                Pesan(salah, MessageBoxIcon.Warning);
                return;
            }

            Bitmap baru;
            Cursor = Cursors.WaitCursor;
            try
            {
                baru = PembuatQr.Buat(main, secret, Signing, kunci, Level, ukuran);
            }
            catch (Exception ex)
            {
                Pesan("QR gagal dibuat: " + ex.Message, MessageBoxIcon.Error);
                return;
            }
            finally
            {
                Cursor = Cursors.Default;
            }

            if (gambar != null) gambar.Dispose();
            gambar = baru;
            picQr.Image = gambar;
            btnSimpan.Enabled = btnSalinGambar.Enabled = true;
            lblInfoGambar.Text = gambar.Width + " x " + gambar.Height + " px  |  EC " + Level + "  |  "
                                 + (Signing ? "signing key: AKTIF (keyId " + kunci.KeyId + ")" : "signing key: TIDAK AKTIF");

            string laporan;
            bool ok = PembuatQr.Verifikasi(gambar, main, secret, Signing, kunci, out laporan);
            lblVerifikasi.Text = (ok ? "VERIFIKASI OK\r\n" : "VERIFIKASI GAGAL\r\n") + laporan;
            lblVerifikasi.BackColor = ok ? WARNA_OK : WARNA_GAGAL;
        }

        private void btnSimpan_Click(object sender, EventArgs e)
        {
            if (gambar == null) return;
            using (var dlg = new SaveFileDialog())
            {
                dlg.Filter = "Gambar PNG (*.png)|*.png";
                dlg.FileName = NamaFile(txtMain.Text) + (Signing ? "_signed" : "_plain") + ".png";
                if (Directory.Exists(setel.FolderSimpan)) dlg.InitialDirectory = setel.FolderSimpan;
                if (dlg.ShowDialog(this) != DialogResult.OK) return;
                gambar.Save(dlg.FileName, ImageFormat.Png);
                setel.FolderSimpan = Path.GetDirectoryName(dlg.FileName);
            }
        }

        private void btnSalinGambar_Click(object sender, EventArgs e)
        {
            if (gambar != null) Clipboard.SetImage(gambar);
        }

        // ---------------- Kunci ----------------

        private void btnPilihKunci_Click(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog())
            {
                dlg.Filter = "Kunci qrsec (*.key)|*.key|Semua file|*.*";
                if (File.Exists(txtKunci.Text)) dlg.InitialDirectory = Path.GetDirectoryName(txtKunci.Text);
                if (dlg.ShowDialog(this) == DialogResult.OK)
                    MuatKunci(dlg.FileName, false);
            }
        }

        private void btnBuatKunci_Click(object sender, EventArgs e)
        {
            byte keyId = (byte)numKeyId.Value;
            using (var dlg = new SaveFileDialog())
            {
                dlg.Filter = "Kunci qrsec (*.key)|*.key";
                dlg.FileName = "qrsec_" + keyId.ToString("00") + ".key";
                dlg.Title = "Simpan kunci privat baru (keyId " + keyId + ")";
                if (dlg.ShowDialog(this) != DialogResult.OK) return;
                if (File.Exists(dlg.FileName) && MessageBox.Show(this,
                        "File kunci ini akan DITIMPA.\nQR yang sudah dicetak dengan kunci lama tetap sah selama APK masih punya public key lamanya, " +
                        "tetapi Anda tidak bisa lagi mencetak dengan kunci lama.\n\nLanjutkan?",
                        "Timpa kunci?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                    return;
                try
                {
                    using (var baru = KunciQr.Buat(keyId))
                    {
                        baru.SimpanPrivat(dlg.FileName);
                        File.WriteAllText(dlg.FileName + ".pub.txt", baru.PublicKeyBase64);
                    }
                }
                catch (Exception ex)
                {
                    Pesan("Kunci gagal dibuat: " + ex.Message, MessageBoxIcon.Error);
                    return;
                }
                MuatKunci(dlg.FileName, true);
                Clipboard.SetText(PembuatQr.BarisJava(kunci));
                Pesan("Kunci keyId " + keyId + " dibuat:\n" + dlg.FileName +
                      "\n\nPublic key juga disimpan di " + Path.GetFileName(dlg.FileName) + ".pub.txt dan SUDAH DISALIN ke clipboard " +
                      "dalam bentuk baris Java:\n\n" + PembuatQr.BarisJava(kunci) +
                      "\n\nTempel di QrSec.java (blok PUBLIC_KEYS) di project APK, lalu build ulang APK." +
                      "\n\nSimpan cadangan file .key di tempat aman; file ini hanya bisa dibuka oleh user Windows ini.",
                      MessageBoxIcon.Information);
            }
        }

        private void btnSalinPublik_Click(object sender, EventArgs e)
        {
            if (kunci == null) return;
            Clipboard.SetText(PembuatQr.BarisJava(kunci));
            Pesan("Disalin ke clipboard:\n\n" + PembuatQr.BarisJava(kunci) +
                  "\n\nTempel di QrSec.java (blok PUBLIC_KEYS) di project APK.", MessageBoxIcon.Information);
        }

        private void MuatKunci(string path, bool diam)
        {
            KunciQr k;
            try
            {
                k = KunciQr.MuatPrivat(path);
            }
            catch (CryptographicException)
            {
                if (!diam)
                    Pesan("Kunci tidak bisa dibuka.\nFile .key dienkripsi DPAPI dan hanya bisa dibuka oleh user Windows yang membuatnya.", MessageBoxIcon.Error);
                return;
            }
            catch (Exception ex)
            {
                if (!diam) Pesan("Kunci tidak bisa dibuka: " + ex.Message, MessageBoxIcon.Error);
                return;
            }
            if (kunci != null) kunci.Dispose();
            kunci = k;
            txtKunci.Text = path;
            setel.FileKunci = path;
            string pub = k.PublicKeyBase64;
            lblInfoKunci.Text = "Kunci keyId " + k.KeyId + " dimuat  |  public …" + pub.Substring(pub.Length - 16);
            lblInfoKunci.ForeColor = Color.DarkGreen;
            AturSigning();
            Isi_TextChanged(this, EventArgs.Empty);
        }

        // ---------------- Bantu ----------------

        private void Pesan(string teks, MessageBoxIcon ikon)
        {
            MessageBox.Show(this, teks, Text, MessageBoxButtons.OK, ikon);
        }

        private static string NamaFile(string main)
        {
            var bersih = new string((main ?? "").Where(c => char.IsLetterOrDigit(c) || c == '-' || c == '_').Take(40).ToArray());
            return bersih.Length > 0 ? bersih : "qrsec";
        }
    }
}
