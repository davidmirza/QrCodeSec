using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using zvr;
using zvr.Common;
using zvr.QrCode;
using zvr.QrCode.Internal;

namespace mrzha
{
    public class Buat
    {
        public static Bitmap Cetak(String Main, String Secreet,String Level, int Size)
        {
            zvr.BarcodeWriter barcodeWriter = new BarcodeWriter();
            barcodeWriter.Format = BarcodeFormat.QR_CODE;
            barcodeWriter.Options = (EncodingOptions)Opsi(Level, Size);
            BitMatrix bitMatrix = barcodeWriter.Encode(Main, Secreet);
            return KeBitmap(bitMatrix);
        }

        /// <summary>
        /// Seperti Cetak, tetapi rahasia dibungkus tanda tangan ECDSA (lihat TandaRahasia).
        /// Kunci harus punya kunci privat (KunciQr.MuatPrivat). Rahasia maks TandaRahasia.MAX_RAHASIA_BYTES byte UTF-8.
        /// </summary>
        public static Bitmap CetakAman(String Main, String Secreet, KunciQr Kunci, String Level, int Size)
        {
            byte[] amplop = TandaRahasia.Bungkus(Kunci, Main, Secreet);
            QrCodeEncodingOptions opsi = Opsi(Level, Size);
            BitMatrix bitMatrix = new QRCodeWriter().encode(Main, amplop, BarcodeFormat.QR_CODE, opsi.Width, opsi.Height, opsi.Hints);
            return KeBitmap(bitMatrix);
        }

        private static QrCodeEncodingOptions Opsi(String Level, int Size)
        {
            QrCodeEncodingOptions codeEncodingOptions1 = new QrCodeEncodingOptions();
            codeEncodingOptions1.CharacterSet = "UTF-8";
            if(Level=="L")
            codeEncodingOptions1.ErrorCorrection = ErrorCorrectionLevel.L;
            else if (Level == "M")
                codeEncodingOptions1.ErrorCorrection = ErrorCorrectionLevel.M;
            else if (Level == "H")
                codeEncodingOptions1.ErrorCorrection = ErrorCorrectionLevel.H;
            else
                codeEncodingOptions1.ErrorCorrection = ErrorCorrectionLevel.Q;

            codeEncodingOptions1.Height = Size;
            codeEncodingOptions1.Width = Size;
            return codeEncodingOptions1;
        }

        private static Bitmap KeBitmap(BitMatrix bitMatrix)
        {
            int num = 1;
            Bitmap bitmap = new Bitmap(bitMatrix.Width * num, bitMatrix.Height * num);
            for (int index1 = 0; index1 < bitMatrix.Height; ++index1)
            {
                for (int index2 = 0; index2 < bitMatrix.Width; ++index2)
                {
                    Color color = bitMatrix[index1, index2] ? Color.Black : Color.White;
                    for (int index3 = 0; index3 < num; ++index3)
                    {
                        for (int index4 = 0; index4 < num; ++index4)
                            bitmap.SetPixel(index1 * num + index3, index2 * num + index4, color);
                    }
                }
            }
            return bitmap;
        }
    }

}
