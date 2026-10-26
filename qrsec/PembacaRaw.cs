using System;

namespace mrzha
{
    /// <summary>
    /// Pembaca sisi decoder: ambil main + rahasia dari codeword data QR
    /// (ZXing: Result.RawBytes / Android: result.getRawBytes()).
    /// Ini juga spesifikasi acuan untuk diport ke APK.
    ///
    /// Aliran bit: [ECI 0111 + 8 bit]? [mode 0100][jumlah byte main: 8 bit (versi 1-9) / 16 bit (versi 10+)]
    ///             [main ...][0x00][panjang rahasia + 33][rahasia ...][padding]
    /// Versi QR tidak selalu tersedia dari decoder, jadi 8 dan 16 bit dicoba; struktur divalidasi.
    /// </summary>
    public static class PembacaRaw
    {
        public static bool Ambil(byte[] rawBytes, out byte[] main, out byte[] rahasia)
        {
            return Coba(rawBytes, 8, out main, out rahasia) || Coba(rawBytes, 16, out main, out rahasia);
        }

        private static bool Coba(byte[] raw, int countBits, out byte[] main, out byte[] rahasia)
        {
            main = null;
            rahasia = null;
            if (raw == null || raw.Length < 3)
                return false;
            var r = new BitReader(raw);

            int mode = r.Read(4);
            if (mode == 0x7) // ECI
            {
                if (r.Sisa < 12)
                    return false;
                r.Read(8);
                mode = r.Read(4);
            }
            if (mode != 0x4 || r.Sisa < countBits)
                return false;

            int nMain = r.Read(countBits);
            if (r.Sisa < (nMain + 2) * 8)
                return false;
            byte[] m = new byte[nMain];
            for (int i = 0; i < nMain; i++)
                m[i] = (byte)r.Read(8);

            if (r.Read(8) != 0x00)
                return false;
            int nRahasia = r.Read(8) - zvr.QrCode.Internal.Encoder.RAHASIA_LENGTH_OFFSET;
            if (nRahasia < 0 || r.Sisa < nRahasia * 8)
                return false;
            byte[] s = new byte[nRahasia];
            for (int i = 0; i < nRahasia; i++)
                s[i] = (byte)r.Read(8);

            main = m;
            rahasia = s;
            return true;
        }

        private sealed class BitReader
        {
            private readonly byte[] data;
            private int pos;

            public BitReader(byte[] data)
            {
                this.data = data;
            }

            public int Sisa
            {
                get { return data.Length * 8 - pos; }
            }

            public int Read(int n)
            {
                if (n > Sisa)
                    throw new ArgumentOutOfRangeException("n");
                int v = 0;
                for (int i = 0; i < n; i++, pos++)
                    v = (v << 1) | ((data[pos >> 3] >> (7 - (pos & 7))) & 1);
                return v;
            }
        }
    }
}
