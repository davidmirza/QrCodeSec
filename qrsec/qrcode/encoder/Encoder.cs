// Decompiled with JetBrains decompiler
// Type: zvr.QrCode.Internal.Encoder
// Assembly: qrsec, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 69E797CF-1B7B-4FFE-BC11-3A55EA0E6274
// Assembly location: C:\Users\it\Desktop\qrsec.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using zvr.Common;
using zvr.Common.ReedSolomon;

namespace zvr.QrCode.Internal
{
    public static class Encoder
    {
        private static readonly int[] ALPHANUMERIC_TABLE = new int[96]
        {
      -1,
      -1,
      -1,
      -1,
      -1,
      -1,
      -1,
      -1,
      -1,
      -1,
      -1,
      -1,
      -1,
      -1,
      -1,
      -1,
      -1,
      -1,
      -1,
      -1,
      -1,
      -1,
      -1,
      -1,
      -1,
      -1,
      -1,
      -1,
      -1,
      -1,
      -1,
      -1,
      36,
      -1,
      -1,
      -1,
      37,
      38,
      -1,
      -1,
      -1,
      -1,
      39,
      40,
      -1,
      41,
      42,
      43,
      0,
      1,
      2,
      3,
      4,
      5,
      6,
      7,
      8,
      9,
      44,
      -1,
      -1,
      -1,
      -1,
      -1,
      -1,
      10,
      11,
      12,
      13,
      14,
      15,
      16,
      17,
      18,
      19,
      20,
      21,
      22,
      23,
      24,
      25,
      26,
      27,
      28,
      29,
      30,
      31,
      32,
      33,
      34,
      35,
      -1,
      -1,
      -1,
      -1,
      -1
        };
        internal static string DEFAULT_BYTE_MODE_ENCODING = "ISO-8859-1";

        private static int calculateMaskPenalty(ByteMatrix matrix)
        {
            return MaskUtil.applyMaskPenaltyRule1(matrix) + MaskUtil.applyMaskPenaltyRule2(matrix) + MaskUtil.applyMaskPenaltyRule3(matrix) + MaskUtil.applyMaskPenaltyRule4(matrix);
        }

        public static QRCode encode(string content, string rahasia, ErrorCorrectionLevel ecLevel)
        {
            return Encoder.encode(content, rahasia, ecLevel, (IDictionary<EncodeHintType, object>)null);
        }

        public static QRCode encode(string content, string rahasia, ErrorCorrectionLevel ecLevel, IDictionary<EncodeHintType, object> hints)
        {
            return Encoder.encode(content, Encoder.getBytes(rahasia ?? "", Encoder.getCharacterSet(hints)), ecLevel, hints);
        }

        /// <summary>
        /// Rahasia dalam bentuk byte (boleh biner, mis. amplop bertanda tangan dari TandaRahasia).
        /// Maksimal MAX_RAHASIA_BYTES karena panjangnya disimpan di 1 byte (panjang + 33).
        /// </summary>
        public static QRCode encode(string content, byte[] rahasia, ErrorCorrectionLevel ecLevel, IDictionary<EncodeHintType, object> hints)
        {
            if (rahasia == null)
                rahasia = new byte[0];
            if (rahasia.Length > Encoder.MAX_RAHASIA_BYTES)
                throw new WriterException("Rahasia terlalu panjang: " + rahasia.Length + " byte (maks " + Encoder.MAX_RAHASIA_BYTES + ")");
            bool flag1 = hints != null && hints.ContainsKey(EncodeHintType.CHARACTER_SET);
            string str = Encoder.getCharacterSet(hints);
            bool flag2 = flag1 || !Encoder.DEFAULT_BYTE_MODE_ENCODING.Equals(str);
            // fix: data selalu ditulis 8-bit (append8BitBytes), jadi header mode juga harus BYTE.
            // Dulu chooseMode bisa memilih NUMERIC/ALPHANUMERIC (mis. "BBBHWF0504841") -> scanner baca sampah.
            Mode mode = Mode.BYTE;
            byte[] mainBytes = Encoder.getBytes(content, str);
            BitArray bitArray1 = new BitArray();
            if (mode == Mode.BYTE && flag2)
            {
                CharacterSetECI characterSetEciByName = CharacterSetECI.getCharacterSetECIByName(str);
                if (characterSetEciByName != null && (hints == null || !hints.ContainsKey(EncodeHintType.DISABLE_ECI) || hints[EncodeHintType.DISABLE_ECI] == null || !Convert.ToBoolean(hints[EncodeHintType.DISABLE_ECI].ToString())))
                    Encoder.appendECI(characterSetEciByName, bitArray1);
            }
            if (hints != null && hints.ContainsKey(EncodeHintType.GS1_FORMAT) && hints[EncodeHintType.GS1_FORMAT] != null && Convert.ToBoolean(hints[EncodeHintType.GS1_FORMAT].ToString()))
                Encoder.appendModeInfo(Mode.FNC1_FIRST_POSITION, bitArray1);
            Encoder.appendModeInfo(mode, bitArray1);
            BitArray bitArray2 = new BitArray();
            Encoder.append8BitBytes(mainBytes, rahasia, bitArray2);
            Version version;
            if (hints != null && hints.ContainsKey(EncodeHintType.QR_VERSION))
            {
                version = Version.getVersionForNumber(int.Parse(hints[EncodeHintType.QR_VERSION].ToString()));
                if (!Encoder.willFit(Encoder.calculateBitsNeeded(mode, bitArray1, bitArray2, version), version, ecLevel))
                    throw new WriterException("Data too big for requested version");
            }
            else
                version = Encoder.recommendVersion(ecLevel, mode, bitArray1, bitArray2);
            BitArray bits = new BitArray();
            bits.appendBitArray(bitArray1);
            // fix: panjang = jumlah BYTE main (dulu SizeInBytes - (rahasia.Length + 2), salah bila rahasia non-ASCII)
            Encoder.appendLengthInfo(mainBytes.Length, version, mode, bits);
            bits.appendBitArray(bitArray2);
            Version.ECBlocks ecBlocksForLevel = version.getECBlocksForLevel(ecLevel);
            int numDataBytes = version.TotalCodewords - ecBlocksForLevel.TotalECCodewords;
            Encoder.terminateBits(numDataBytes, bits);
            BitArray bitArray3 = Encoder.interleaveWithECBytes(bits, version.TotalCodewords, numDataBytes, ecBlocksForLevel.NumBlocks);
            QRCode qrCode = new QRCode()
            {
                ECLevel = ecLevel,
                Mode = mode,
                Version = version
            };
            int dimensionForVersion = version.DimensionForVersion;
            ByteMatrix matrix = new ByteMatrix(dimensionForVersion, dimensionForVersion);
            int maskPattern1 = -1;
            if (hints != null && hints.ContainsKey(EncodeHintType.QR_MASK_PATTERN))
            {
                int maskPattern2 = int.Parse(hints[EncodeHintType.QR_MASK_PATTERN].ToString());
                maskPattern1 = QRCode.isValidMaskPattern(maskPattern2) ? maskPattern2 : -1;
            }
            if (maskPattern1 == -1)
                maskPattern1 = Encoder.chooseMaskPattern(bitArray3, ecLevel, version, matrix);
            qrCode.MaskPattern = maskPattern1;
            MatrixUtil.buildMatrix(bitArray3, ecLevel, version, maskPattern1, matrix);
            qrCode.Matrix = matrix;
            return qrCode;
        }

        private static Version recommendVersion(ErrorCorrectionLevel ecLevel, Mode mode, BitArray headerBits, BitArray dataBits)
        {
            Version version = Encoder.chooseVersion(Encoder.calculateBitsNeeded(mode, headerBits, dataBits, Version.getVersionForNumber(1)), ecLevel);
            return Encoder.chooseVersion(Encoder.calculateBitsNeeded(mode, headerBits, dataBits, version), ecLevel);
        }

        private static int calculateBitsNeeded(Mode mode, BitArray headerBits, BitArray dataBits, Version version)
        {
            return headerBits.Size + mode.getCharacterCountBits(version) + dataBits.Size;
        }

        internal static int getAlphanumericCode(int code)
        {
            if (code < Encoder.ALPHANUMERIC_TABLE.Length)
                return Encoder.ALPHANUMERIC_TABLE[code];
            return -1;
        }

        public static Mode chooseMode(string content)
        {
            return Encoder.chooseMode(content, (string)null);
        }

        private static Mode chooseMode(string content, string encoding)
        {
            if ("Shift_JIS".Equals(encoding) && Encoder.isOnlyDoubleByteKanji(content))
                return Mode.KANJI;
            bool flag1 = false;
            bool flag2 = false;
            for (int index = 0; index < content.Length; ++index)
            {
                char ch = content[index];
                if (ch >= '0' && ch <= '9')
                {
                    flag1 = true;
                }
                else
                {
                    if (Encoder.getAlphanumericCode((int)ch) == -1)
                        return Mode.BYTE;
                    flag2 = true;
                }
            }
            if (flag2)
                return Mode.ALPHANUMERIC;
            if (flag1)
                return Mode.NUMERIC;
            return Mode.BYTE;
        }

        private static bool isOnlyDoubleByteKanji(string content)
        {
            byte[] bytes;
            try
            {
                bytes = Encoding.GetEncoding("Shift_JIS").GetBytes(content);
            }
            catch (Exception ex)
            {
                return false;
            }
            int length = bytes.Length;
            if (length % 2 != 0)
                return false;
            int index = 0;
            while (index < length)
            {
                int num = (int)bytes[index] & (int)byte.MaxValue;
                if ((num < 129 || num > 159) && (num < 224 || num > 235))
                    return false;
                index += 2;
            }
            return true;
        }

        private static int chooseMaskPattern(BitArray bits, ErrorCorrectionLevel ecLevel, Version version, ByteMatrix matrix)
        {
            int num1 = int.MaxValue;
            int num2 = -1;
            for (int maskPattern = 0; maskPattern < QRCode.NUM_MASK_PATTERNS; ++maskPattern)
            {
                MatrixUtil.buildMatrix(bits, ecLevel, version, maskPattern, matrix);
                int maskPenalty = Encoder.calculateMaskPenalty(matrix);
                if (maskPenalty < num1)
                {
                    num1 = maskPenalty;
                    num2 = maskPattern;
                }
            }
            return num2;
        }

        private static Version chooseVersion(int numInputBits, ErrorCorrectionLevel ecLevel)
        {
            for (int versionNumber = 1; versionNumber <= 40; ++versionNumber)
            {
                Version versionForNumber = Version.getVersionForNumber(versionNumber);
                if (Encoder.willFit(numInputBits, versionForNumber, ecLevel))
                    return versionForNumber;
            }
            throw new WriterException("Data too big");
        }

        private static bool willFit(int numInputBits, Version version, ErrorCorrectionLevel ecLevel)
        {
            return version.TotalCodewords - version.getECBlocksForLevel(ecLevel).TotalECCodewords >= (numInputBits + 7) / 8;
        }

        internal static void terminateBits(int numDataBytes, BitArray bits)
        {
            //jumat
         //   Console.WriteLine("panjang " + numDataBytes);
            int num1 = numDataBytes << 3;
            if (bits.Size > num1)
                throw new WriterException("data bits cannot fit in the QR Code" + (object)bits.Size + " > " + (object)num1);
            for (int index = 0; index < 4 && bits.Size < num1; ++index)
                bits.appendBit(false);
            int num2 = bits.Size & 7;
            if (num2 > 0)
            {
                for (int index = num2; index < 8; ++index)
                    bits.appendBit(false);
            }
            int num3 = numDataBytes - bits.SizeInBytes;
            for (int index = 0; index < num3; ++index)
                bits.appendBits((index & 1) == 0 ? 236 : 17, 8);
            if (bits.Size != num1)
                throw new WriterException("Bits size does not equal capacity");
        }

        internal static void getNumDataBytesAndNumECBytesForBlockID(int numTotalBytes, int numDataBytes, int numRSBlocks, int blockID, int[] numDataBytesInBlock, int[] numECBytesInBlock)
        {
            if (blockID >= numRSBlocks)
                throw new WriterException("Block ID too large");
            int num1 = numTotalBytes % numRSBlocks;
            int num2 = numRSBlocks - num1;
            int num3 = numTotalBytes / numRSBlocks;
            int num4 = num3 + 1;
            int num5 = numDataBytes / numRSBlocks;
            int num6 = num5 + 1;
            int num7 = num3 - num5;
            int num8 = num4 - num6;
            if (num7 != num8)
                throw new WriterException("EC bytes mismatch");
            if (numRSBlocks != num2 + num1)
                throw new WriterException("RS blocks mismatch");
            if (numTotalBytes != (num5 + num7) * num2 + (num6 + num8) * num1)
                throw new WriterException("Total bytes mismatch");
            if (blockID < num2)
            {
                numDataBytesInBlock[0] = num5;
                numECBytesInBlock[0] = num7;
            }
            else
            {
                numDataBytesInBlock[0] = num6;
                numECBytesInBlock[0] = num8;
            }
        }

        internal static BitArray interleaveWithECBytes(BitArray bits, int numTotalBytes, int numDataBytes, int numRSBlocks)
        {
            if (bits.SizeInBytes != numDataBytes)
                throw new WriterException("Number of bits and data bytes does not match");
            int num = 0;
            int val1_1 = 0;
            int val1_2 = 0;
            List<BlockPair> blockPairList = new List<BlockPair>(numRSBlocks);
            for (int blockID = 0; blockID < numRSBlocks; ++blockID)
            {
                int[] numDataBytesInBlock = new int[1];
                int[] numECBytesInBlock = new int[1];
                Encoder.getNumDataBytesAndNumECBytesForBlockID(numTotalBytes, numDataBytes, numRSBlocks, blockID, numDataBytesInBlock, numECBytesInBlock);
                int length = numDataBytesInBlock[0];
                byte[] numArray = new byte[length];
                bits.toBytes(8 * num, numArray, 0, length);
                byte[] ecBytes = Encoder.generateECBytes(numArray, numECBytesInBlock[0]);
                blockPairList.Add(new BlockPair(numArray, ecBytes));
                val1_1 = Math.Max(val1_1, length);
                val1_2 = Math.Max(val1_2, ecBytes.Length);
                num += numDataBytesInBlock[0];
            }
            if (numDataBytes != num)
                throw new WriterException("Data bytes does not match offset");
            BitArray bitArray = new BitArray();
            for (int index = 0; index < val1_1; ++index)
            {
                foreach (BlockPair blockPair in blockPairList)
                {
                    byte[] dataBytes = blockPair.DataBytes;
                    if (index < dataBytes.Length)
                        bitArray.appendBits((int)dataBytes[index], 8);
                }
            }
            for (int index = 0; index < val1_2; ++index)
            {
                foreach (BlockPair blockPair in blockPairList)
                {
                    byte[] errorCorrectionBytes = blockPair.ErrorCorrectionBytes;
                    if (index < errorCorrectionBytes.Length)
                        bitArray.appendBits((int)errorCorrectionBytes[index], 8);
                }
            }
            if (numTotalBytes != bitArray.SizeInBytes)
                throw new WriterException("Interleaving error: " + (object)numTotalBytes + " and " + (object)bitArray.SizeInBytes + " differ.");
            return bitArray;
        }

        internal static byte[] generateECBytes(byte[] dataBytes, int numEcBytesInBlock)
        {
            int length = dataBytes.Length;
            int[] toEncode = new int[length + numEcBytesInBlock];
            for (int index = 0; index < length; ++index)
                toEncode[index] = (int)dataBytes[index] & (int)byte.MaxValue;
            new ReedSolomonEncoder(GenericGF.QR_CODE_FIELD_256).encode(toEncode, numEcBytesInBlock);
            byte[] numArray = new byte[numEcBytesInBlock];
            for (int index = 0; index < numEcBytesInBlock; ++index)
                numArray[index] = (byte)toEncode[length + index];
            return numArray;
        }

        internal static void appendModeInfo(Mode mode, BitArray bits)
        {
            bits.appendBits(mode.Bits, 4);
        }

        internal static void appendLengthInfo(int numLetters, Version version, Mode mode, BitArray bits)
        {
            Debug.WriteLine("panjang kata: " + (object)numLetters);
            int characterCountBits = mode.getCharacterCountBits(version);
            if (numLetters >= 1 << characterCountBits)
                throw new WriterException(numLetters.ToString() + " is bigger than " + (object)((1 << characterCountBits) - 1));
            bits.appendBits(numLetters, characterCountBits);
        }

        internal static void appendNumericBytes(string content, BitArray bits)
        {
            int length = content.Length;
            int index = 0;
            while (index < length)
            {
                int num1 = (int)content[index] - 48;
                if (index + 2 < length)
                {
                    int num2 = (int)content[index + 1] - 48;
                    int num3 = (int)content[index + 2] - 48;
                    bits.appendBits(num1 * 100 + num2 * 10 + num3, 10);
                    index += 3;
                }
                else if (index + 1 < length)
                {
                    int num2 = (int)content[index + 1] - 48;
                    bits.appendBits(num1 * 10 + num2, 7);
                    index += 2;
                }
                else
                {
                    bits.appendBits(num1, 4);
                    ++index;
                }
            }
        }

        internal static void appendAlphanumericBytes(string content, BitArray bits)
        {
            int length = content.Length;
            int index = 0;
            while (index < length)
            {
                int alphanumericCode1 = Encoder.getAlphanumericCode((int)content[index]);
                if (alphanumericCode1 == -1)
                    throw new WriterException();
                if (index + 1 < length)
                {
                    int alphanumericCode2 = Encoder.getAlphanumericCode((int)content[index + 1]);
                    if (alphanumericCode2 == -1)
                        throw new WriterException();
                    bits.appendBits(alphanumericCode1 * 45 + alphanumericCode2, 11);
                    index += 2;
                }
                else
                {
                    bits.appendBits(alphanumericCode1, 6);
                    ++index;
                }
            }
        }

        /// <summary>
        /// Layout: [main...][0x00][panjang rahasia + 33][rahasia...]
        /// 0x00 terbaca sebagai terminator oleh scanner biasa, jadi mereka hanya melihat main.
        /// </summary>
        internal static void append8BitBytes(byte[] mainBytes, byte[] secret, BitArray bits)
        {
            for (int index = 0; index < mainBytes.Length; ++index)
                bits.appendBits((int)mainBytes[index], 8);
            bits.appendBits(0, 8);
            bits.appendBits(secret.Length + RAHASIA_LENGTH_OFFSET, 8);
            for (int index = 0; index < secret.Length; ++index)
                bits.appendBits((int)secret[index], 8);
        }

        /// <summary>byte penanda panjang = panjang rahasia + 33 (format lama: (len + 1) + 32)</summary>
        public const int RAHASIA_LENGTH_OFFSET = 33;
        public const int MAX_RAHASIA_BYTES = 255 - RAHASIA_LENGTH_OFFSET;

        internal static string getCharacterSet(IDictionary<EncodeHintType, object> hints)
        {
            return (hints == null || !hints.ContainsKey(EncodeHintType.CHARACTER_SET) ? (string)null : (string)hints[EncodeHintType.CHARACTER_SET]) ?? Encoder.DEFAULT_BYTE_MODE_ENCODING;
        }

        internal static byte[] getBytes(string text, string encoding)
        {
            try
            {
                return Encoding.GetEncoding(encoding).GetBytes(text);
            }
            catch (Exception ex)
            {
                throw new WriterException(ex.Message, ex);
            }
        }

        internal static void appendKanjiBytes(string content, BitArray bits)
        {
            byte[] bytes;
            try
            {
                bytes = Encoding.GetEncoding("Shift_JIS").GetBytes(content);
            }
            catch (Exception ex)
            {
                throw new WriterException(ex.Message, ex);
            }
            if (bytes.Length % 2 != 0)
                throw new WriterException("Kanji byte size not even");
            int num1 = bytes.Length - 1;
            int index = 0;
            while (index < num1)
            {
                int num2 = ((int)bytes[index] & (int)byte.MaxValue) << 8 | (int)bytes[index + 1] & (int)byte.MaxValue;
                int num3 = -1;
                if (num2 >= 33088 && num2 <= 40956)
                    num3 = num2 - 33088;
                else if (num2 >= 57408 && num2 <= 60351)
                    num3 = num2 - 49472;
                if (num3 == -1)
                    throw new WriterException("Invalid byte sequence");
                int num4 = (num3 >> 8) * 192 + (num3 & (int)byte.MaxValue);
                bits.appendBits(num4, 13);
                index += 2;
            }
        }

        private static void appendECI(CharacterSetECI eci, BitArray bits)
        {
            bits.appendBits(Mode.ECI.Bits, 4);
            bits.appendBits(eci.Value, 8);
        }
    }
}
