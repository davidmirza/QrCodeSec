namespace QrSecDesigner
{
    partial class FormUtama
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.lblMain = new System.Windows.Forms.Label();
            this.txtMain = new System.Windows.Forms.TextBox();
            this.lblSecret = new System.Windows.Forms.Label();
            this.txtSecret = new System.Windows.Forms.TextBox();
            this.lblHitung = new System.Windows.Forms.Label();
            this.grpSetup = new System.Windows.Forms.GroupBox();
            this.btnSalinPublik = new System.Windows.Forms.Button();
            this.lblInfoKunci = new System.Windows.Forms.Label();
            this.btnBuatKunci = new System.Windows.Forms.Button();
            this.numKeyId = new System.Windows.Forms.NumericUpDown();
            this.lblKeyIdBaru = new System.Windows.Forms.Label();
            this.btnPilihKunci = new System.Windows.Forms.Button();
            this.txtKunci = new System.Windows.Forms.TextBox();
            this.lblKunci = new System.Windows.Forms.Label();
            this.cmbUkuran = new System.Windows.Forms.ComboBox();
            this.lblUkuran = new System.Windows.Forms.Label();
            this.cmbEc = new System.Windows.Forms.ComboBox();
            this.lblEc = new System.Windows.Forms.Label();
            this.chkSigning = new System.Windows.Forms.CheckBox();
            this.btnBuat = new System.Windows.Forms.Button();
            this.btnSimpan = new System.Windows.Forms.Button();
            this.btnSalinGambar = new System.Windows.Forms.Button();
            this.lblVerifikasi = new System.Windows.Forms.Label();
            this.picQr = new System.Windows.Forms.PictureBox();
            this.lblInfoGambar = new System.Windows.Forms.Label();
            this.tip = new System.Windows.Forms.ToolTip(this.components);
            this.grpSetup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numKeyId)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picQr)).BeginInit();
            this.SuspendLayout();
            //
            // lblMain
            //
            this.lblMain.AutoSize = true;
            this.lblMain.Location = new System.Drawing.Point(12, 12);
            this.lblMain.Name = "lblMain";
            this.lblMain.Size = new System.Drawing.Size(196, 15);
            this.lblMain.TabIndex = 0;
            this.lblMain.Text = "Isi QR / main (terbaca semua scanner):";
            //
            // txtMain
            //
            this.txtMain.Location = new System.Drawing.Point(12, 32);
            this.txtMain.Multiline = true;
            this.txtMain.Name = "txtMain";
            this.txtMain.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtMain.Size = new System.Drawing.Size(440, 56);
            this.txtMain.TabIndex = 1;
            this.tip.SetToolTip(this.txtMain, "Teks yang terlihat oleh semua aplikasi scanner, mis. nomor seri atau URL.");
            this.txtMain.TextChanged += new System.EventHandler(this.Isi_TextChanged);
            //
            // lblSecret
            //
            this.lblSecret.AutoSize = true;
            this.lblSecret.Location = new System.Drawing.Point(12, 98);
            this.lblSecret.Name = "lblSecret";
            this.lblSecret.Size = new System.Drawing.Size(218, 15);
            this.lblSecret.TabIndex = 2;
            this.lblSecret.Text = "Secret (hanya terbaca aplikasi qrsec):";
            //
            // txtSecret
            //
            this.txtSecret.Location = new System.Drawing.Point(12, 118);
            this.txtSecret.Multiline = true;
            this.txtSecret.Name = "txtSecret";
            this.txtSecret.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtSecret.Size = new System.Drawing.Size(440, 56);
            this.txtSecret.TabIndex = 3;
            this.tip.SetToolTip(this.txtSecret, "Disimpan setelah terminator QR; scanner biasa tidak menampilkannya.");
            this.txtSecret.TextChanged += new System.EventHandler(this.Isi_TextChanged);
            //
            // lblHitung
            //
            this.lblHitung.AutoSize = true;
            this.lblHitung.Location = new System.Drawing.Point(12, 178);
            this.lblHitung.Name = "lblHitung";
            this.lblHitung.Size = new System.Drawing.Size(120, 15);
            this.lblHitung.TabIndex = 4;
            this.lblHitung.Text = "Secret: 0 / 156 byte";
            //
            // grpSetup
            //
            this.grpSetup.Controls.Add(this.btnSalinPublik);
            this.grpSetup.Controls.Add(this.lblInfoKunci);
            this.grpSetup.Controls.Add(this.btnBuatKunci);
            this.grpSetup.Controls.Add(this.numKeyId);
            this.grpSetup.Controls.Add(this.lblKeyIdBaru);
            this.grpSetup.Controls.Add(this.btnPilihKunci);
            this.grpSetup.Controls.Add(this.txtKunci);
            this.grpSetup.Controls.Add(this.lblKunci);
            this.grpSetup.Controls.Add(this.cmbUkuran);
            this.grpSetup.Controls.Add(this.lblUkuran);
            this.grpSetup.Controls.Add(this.cmbEc);
            this.grpSetup.Controls.Add(this.lblEc);
            this.grpSetup.Controls.Add(this.chkSigning);
            this.grpSetup.Location = new System.Drawing.Point(12, 202);
            this.grpSetup.Name = "grpSetup";
            this.grpSetup.Size = new System.Drawing.Size(440, 258);
            this.grpSetup.TabIndex = 5;
            this.grpSetup.TabStop = false;
            this.grpSetup.Text = "Setup QR";
            //
            // chkSigning
            //
            this.chkSigning.AutoSize = true;
            this.chkSigning.Checked = true;
            this.chkSigning.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSigning.Location = new System.Drawing.Point(12, 24);
            this.chkSigning.Name = "chkSigning";
            this.chkSigning.Size = new System.Drawing.Size(272, 19);
            this.chkSigning.TabIndex = 0;
            this.chkSigning.Text = "Aktifkan signing key (tanda tangan ECDSA)";
            this.tip.SetToolTip(this.chkSigning, "Mati = hanya main + secret polos. APK menampilkan \"Signing key: TIDAK AKTIF\".");
            this.chkSigning.UseVisualStyleBackColor = true;
            this.chkSigning.CheckedChanged += new System.EventHandler(this.chkSigning_CheckedChanged);
            //
            // lblEc
            //
            this.lblEc.AutoSize = true;
            this.lblEc.Location = new System.Drawing.Point(12, 58);
            this.lblEc.Name = "lblEc";
            this.lblEc.Size = new System.Drawing.Size(101, 15);
            this.lblEc.TabIndex = 1;
            this.lblEc.Text = "Error correction:";
            //
            // cmbEc
            //
            this.cmbEc.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEc.FormattingEnabled = true;
            this.cmbEc.Items.AddRange(new object[] {
            "L - pulih 7% (QR paling kecil)",
            "M - pulih 15%",
            "Q - pulih 25% (disarankan)",
            "H - pulih 30% (paling tahan rusak)"});
            this.cmbEc.Location = new System.Drawing.Point(146, 54);
            this.cmbEc.Name = "cmbEc";
            this.cmbEc.Size = new System.Drawing.Size(280, 23);
            this.cmbEc.TabIndex = 2;
            this.cmbEc.SelectedIndexChanged += new System.EventHandler(this.Isi_TextChanged);
            //
            // lblUkuran
            //
            this.lblUkuran.AutoSize = true;
            this.lblUkuran.Location = new System.Drawing.Point(12, 92);
            this.lblUkuran.Name = "lblUkuran";
            this.lblUkuran.Size = new System.Drawing.Size(117, 15);
            this.lblUkuran.TabIndex = 3;
            this.lblUkuran.Text = "Ukuran gambar (px):";
            //
            // cmbUkuran
            //
            this.cmbUkuran.FormattingEnabled = true;
            this.cmbUkuran.Items.AddRange(new object[] {
            "200",
            "300",
            "400",
            "600",
            "800",
            "1000",
            "1200"});
            this.cmbUkuran.Location = new System.Drawing.Point(146, 88);
            this.cmbUkuran.Name = "cmbUkuran";
            this.cmbUkuran.Size = new System.Drawing.Size(100, 23);
            this.cmbUkuran.TabIndex = 4;
            this.cmbUkuran.Text = "300";
            this.tip.SetToolTip(this.cmbUkuran, "100 - 2000 px. Gambar bisa sedikit lebih besar bila QR butuh lebih banyak modul.");
            this.cmbUkuran.TextChanged += new System.EventHandler(this.Isi_TextChanged);
            //
            // lblKunci
            //
            this.lblKunci.AutoSize = true;
            this.lblKunci.Location = new System.Drawing.Point(12, 126);
            this.lblKunci.Name = "lblKunci";
            this.lblKunci.Size = new System.Drawing.Size(101, 15);
            this.lblKunci.TabIndex = 5;
            this.lblKunci.Text = "File kunci (.key):";
            //
            // txtKunci
            //
            this.txtKunci.Location = new System.Drawing.Point(146, 122);
            this.txtKunci.Name = "txtKunci";
            this.txtKunci.ReadOnly = true;
            this.txtKunci.Size = new System.Drawing.Size(196, 23);
            this.txtKunci.TabIndex = 6;
            //
            // btnPilihKunci
            //
            this.btnPilihKunci.Location = new System.Drawing.Point(348, 121);
            this.btnPilihKunci.Name = "btnPilihKunci";
            this.btnPilihKunci.Size = new System.Drawing.Size(78, 25);
            this.btnPilihKunci.TabIndex = 7;
            this.btnPilihKunci.Text = "Pilih...";
            this.btnPilihKunci.UseVisualStyleBackColor = true;
            this.btnPilihKunci.Click += new System.EventHandler(this.btnPilihKunci_Click);
            //
            // lblKeyIdBaru
            //
            this.lblKeyIdBaru.AutoSize = true;
            this.lblKeyIdBaru.Location = new System.Drawing.Point(12, 161);
            this.lblKeyIdBaru.Name = "lblKeyIdBaru";
            this.lblKeyIdBaru.Size = new System.Drawing.Size(104, 15);
            this.lblKeyIdBaru.TabIndex = 8;
            this.lblKeyIdBaru.Text = "KeyId kunci baru:";
            //
            // numKeyId
            //
            this.numKeyId.Location = new System.Drawing.Point(146, 158);
            this.numKeyId.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numKeyId.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numKeyId.Name = "numKeyId";
            this.numKeyId.Size = new System.Drawing.Size(60, 23);
            this.numKeyId.TabIndex = 9;
            this.numKeyId.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            //
            // btnBuatKunci
            //
            this.btnBuatKunci.Location = new System.Drawing.Point(214, 156);
            this.btnBuatKunci.Name = "btnBuatKunci";
            this.btnBuatKunci.Size = new System.Drawing.Size(212, 27);
            this.btnBuatKunci.TabIndex = 10;
            this.btnBuatKunci.Text = "Buat kunci baru...";
            this.tip.SetToolTip(this.btnBuatKunci, "Membuat pasangan kunci ECDSA P-256. File .key terenkripsi DPAPI (hanya user Windows ini).");
            this.btnBuatKunci.UseVisualStyleBackColor = true;
            this.btnBuatKunci.Click += new System.EventHandler(this.btnBuatKunci_Click);
            //
            // lblInfoKunci
            //
            this.lblInfoKunci.Location = new System.Drawing.Point(12, 192);
            this.lblInfoKunci.Name = "lblInfoKunci";
            this.lblInfoKunci.Size = new System.Drawing.Size(414, 22);
            this.lblInfoKunci.TabIndex = 11;
            this.lblInfoKunci.Text = "Kunci: belum dimuat";
            //
            // btnSalinPublik
            //
            this.btnSalinPublik.Location = new System.Drawing.Point(12, 220);
            this.btnSalinPublik.Name = "btnSalinPublik";
            this.btnSalinPublik.Size = new System.Drawing.Size(414, 27);
            this.btnSalinPublik.TabIndex = 12;
            this.btnSalinPublik.Text = "Salin public key untuk APK (QrSec.PUBLIC_KEYS)";
            this.btnSalinPublik.UseVisualStyleBackColor = true;
            this.btnSalinPublik.Click += new System.EventHandler(this.btnSalinPublik_Click);
            //
            // btnBuat
            //
            this.btnBuat.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnBuat.Location = new System.Drawing.Point(12, 470);
            this.btnBuat.Name = "btnBuat";
            this.btnBuat.Size = new System.Drawing.Size(140, 36);
            this.btnBuat.TabIndex = 6;
            this.btnBuat.Text = "Buat QR";
            this.btnBuat.UseVisualStyleBackColor = true;
            this.btnBuat.Click += new System.EventHandler(this.btnBuat_Click);
            //
            // btnSimpan
            //
            this.btnSimpan.Enabled = false;
            this.btnSimpan.Location = new System.Drawing.Point(162, 470);
            this.btnSimpan.Name = "btnSimpan";
            this.btnSimpan.Size = new System.Drawing.Size(140, 36);
            this.btnSimpan.TabIndex = 7;
            this.btnSimpan.Text = "Simpan PNG...";
            this.btnSimpan.UseVisualStyleBackColor = true;
            this.btnSimpan.Click += new System.EventHandler(this.btnSimpan_Click);
            //
            // btnSalinGambar
            //
            this.btnSalinGambar.Enabled = false;
            this.btnSalinGambar.Location = new System.Drawing.Point(312, 470);
            this.btnSalinGambar.Name = "btnSalinGambar";
            this.btnSalinGambar.Size = new System.Drawing.Size(140, 36);
            this.btnSalinGambar.TabIndex = 8;
            this.btnSalinGambar.Text = "Salin gambar";
            this.btnSalinGambar.UseVisualStyleBackColor = true;
            this.btnSalinGambar.Click += new System.EventHandler(this.btnSalinGambar_Click);
            //
            // lblVerifikasi
            //
            this.lblVerifikasi.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lblVerifikasi.BackColor = System.Drawing.Color.WhiteSmoke;
            this.lblVerifikasi.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblVerifikasi.Location = new System.Drawing.Point(12, 516);
            this.lblVerifikasi.Name = "lblVerifikasi";
            this.lblVerifikasi.Padding = new System.Windows.Forms.Padding(6);
            this.lblVerifikasi.Size = new System.Drawing.Size(440, 112);
            this.lblVerifikasi.TabIndex = 9;
            this.lblVerifikasi.Text = "Isi main + secret, pilih setup, lalu klik Buat QR.\r\nQR hasil akan dibaca ulang otomatis untuk verifikasi.";
            //
            // picQr
            //
            this.picQr.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.picQr.BackColor = System.Drawing.Color.White;
            this.picQr.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picQr.Location = new System.Drawing.Point(468, 12);
            this.picQr.Name = "picQr";
            this.picQr.Size = new System.Drawing.Size(480, 590);
            this.picQr.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picQr.TabIndex = 10;
            this.picQr.TabStop = false;
            //
            // lblInfoGambar
            //
            this.lblInfoGambar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblInfoGambar.Location = new System.Drawing.Point(468, 608);
            this.lblInfoGambar.Name = "lblInfoGambar";
            this.lblInfoGambar.Size = new System.Drawing.Size(480, 20);
            this.lblInfoGambar.TabIndex = 11;
            this.lblInfoGambar.Text = "Belum ada QR";
            this.lblInfoGambar.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // FormUtama
            //
            this.AcceptButton = this.btnBuat;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(960, 640);
            this.Controls.Add(this.lblInfoGambar);
            this.Controls.Add(this.picQr);
            this.Controls.Add(this.lblVerifikasi);
            this.Controls.Add(this.btnSalinGambar);
            this.Controls.Add(this.btnSimpan);
            this.Controls.Add(this.btnBuat);
            this.Controls.Add(this.grpSetup);
            this.Controls.Add(this.lblHitung);
            this.Controls.Add(this.txtSecret);
            this.Controls.Add(this.lblSecret);
            this.Controls.Add(this.txtMain);
            this.Controls.Add(this.lblMain);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MinimumSize = new System.Drawing.Size(976, 679);
            this.Name = "FormUtama";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "QrSec Designer - QR + Secret";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormUtama_FormClosing);
            this.Load += new System.EventHandler(this.FormUtama_Load);
            this.grpSetup.ResumeLayout(false);
            this.grpSetup.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numKeyId)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picQr)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblMain;
        private System.Windows.Forms.TextBox txtMain;
        private System.Windows.Forms.Label lblSecret;
        private System.Windows.Forms.TextBox txtSecret;
        private System.Windows.Forms.Label lblHitung;
        private System.Windows.Forms.GroupBox grpSetup;
        private System.Windows.Forms.CheckBox chkSigning;
        private System.Windows.Forms.Label lblEc;
        private System.Windows.Forms.ComboBox cmbEc;
        private System.Windows.Forms.Label lblUkuran;
        private System.Windows.Forms.ComboBox cmbUkuran;
        private System.Windows.Forms.Label lblKunci;
        private System.Windows.Forms.TextBox txtKunci;
        private System.Windows.Forms.Button btnPilihKunci;
        private System.Windows.Forms.Label lblKeyIdBaru;
        private System.Windows.Forms.NumericUpDown numKeyId;
        private System.Windows.Forms.Button btnBuatKunci;
        private System.Windows.Forms.Label lblInfoKunci;
        private System.Windows.Forms.Button btnSalinPublik;
        private System.Windows.Forms.Button btnBuat;
        private System.Windows.Forms.Button btnSimpan;
        private System.Windows.Forms.Button btnSalinGambar;
        private System.Windows.Forms.Label lblVerifikasi;
        private System.Windows.Forms.PictureBox picQr;
        private System.Windows.Forms.Label lblInfoGambar;
        private System.Windows.Forms.ToolTip tip;
    }
}
