using System;
using System.Collections.Generic;
using System.IO;

namespace QrSecDesigner
{
    /// <summary>Setup terakhir disimpan di %APPDATA%\QrSecDesigner\pengaturan.txt (key=value).</summary>
    public sealed class Pengaturan
    {
        public string FileKunci = "";
        public bool Signing = true;
        public string Level = "Q";
        public int Ukuran = 300;
        public string FolderSimpan = "";

        private static string Path_
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "QrSecDesigner", "pengaturan.txt");
            }
        }

        public static Pengaturan Muat()
        {
            var p = new Pengaturan();
            try
            {
                if (!File.Exists(Path_))
                    return p;
                var d = new Dictionary<string, string>();
                foreach (string baris in File.ReadAllLines(Path_))
                {
                    int i = baris.IndexOf('=');
                    if (i > 0)
                        d[baris.Substring(0, i)] = baris.Substring(i + 1);
                }
                string v;
                if (d.TryGetValue("FileKunci", out v)) p.FileKunci = v;
                if (d.TryGetValue("Signing", out v)) p.Signing = v != "0";
                if (d.TryGetValue("Level", out v) && Array.IndexOf(PembuatQr.LEVEL, v) >= 0) p.Level = v;
                int n;
                if (d.TryGetValue("Ukuran", out v) && int.TryParse(v, out n)) p.Ukuran = n;
                if (d.TryGetValue("FolderSimpan", out v)) p.FolderSimpan = v;
            }
            catch (Exception)
            {
                // file rusak -> pakai default
            }
            return p;
        }

        public void Simpan()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(Path_));
                File.WriteAllLines(Path_, new[]
                {
                    "FileKunci=" + FileKunci,
                    "Signing=" + (Signing ? "1" : "0"),
                    "Level=" + Level,
                    "Ukuran=" + Ukuran,
                    "FolderSimpan=" + FolderSimpan
                });
            }
            catch (Exception)
            {
                // tidak fatal
            }
        }
    }
}
