/*
* 2021 http://dzavira.com/qrcode
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*      http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/

using System;
using System.Collections.Generic;

//using zvr.Aztec;
using zvr.Common;
//using zvr.Datamatrix;
//using zvr.OneD;
//using zvr.PDF417;
using zvr.QrCode;

namespace zvr
{
    /// <summary> This is a factory class which finds the appropriate Writer subclass for the BarcodeFormat
    /// requested and encodes the barcode with the supplied contents.
    /// 
    /// </summary>
    /// <author>  dswitkin@google.com (Daniel Switkin)
    /// </author>
    /// <author>www.Redivivus.in (suraj.supekar@redivivus.in) - Ported from zvr Java Source 
    /// </author>
    public sealed class MultiFormatWriter : Writer
    {
        private static readonly IDictionary<BarcodeFormat, Func<Writer>> formatMap;

        static MultiFormatWriter()
        {
            formatMap = new Dictionary<BarcodeFormat, Func<Writer>>
                        {
                          
                           {BarcodeFormat.QR_CODE, () => new QRCodeWriter()}
                          
                        };
        }

        /// <summary>
        /// Gets the collection of supported writers.
        /// </summary>
        public static ICollection<BarcodeFormat> SupportedWriters
        {
            get { return formatMap.Keys; }
        }

        /// <summary>
        /// encode the given data
        /// </summary>
        /// <param name="contents"></param>
        /// <param name="format"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public BitMatrix encode(String contents, string rahasia, BarcodeFormat format, int width, int height)
        {
            return encode(contents,rahasia, format, width, height, null);
        }

        /// <summary>
        /// encode the given data
        /// </summary>
        /// <param name="contents"></param>
        /// <param name="format"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="hints"></param>
        /// <returns></returns>
        public BitMatrix encode(String contents, string rahasia, BarcodeFormat format, int width, int height, IDictionary<EncodeHintType, object> hints)
        {
            if (!formatMap.ContainsKey(format))
                throw new ArgumentException("No encoder available for format " + format);

            return formatMap[format]().encode(contents,rahasia, format, width, height, hints);
        }
    }
}