/* 
 * Copyright (c) 2015 Andrew Johnson
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of 
 * this software and associated documentation files (the "Software"), to deal in 
 * the Software without restriction, including without limitation the rights to use,
 * copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
 * Software, and to permit persons to whom the Software is furnished to do so,
 * subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all 
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
 * FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
 * COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN
 * AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
 * WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Slice
{
    /// <summary>
    /// Extension methods for Bitmaps
    /// </summary>
    public static class BitmapExtensions
    {
        public static Bitmap FilterIPadSnapshot(this Bitmap sourceBitmap)
        {
            if (sourceBitmap == null)
            {
                throw new ArgumentNullException("sourceBitmap");
            }

            byte[] pixelBuffer = GetPixelBuffer(sourceBitmap);

            for (int k = 0; k + 4 < pixelBuffer.Length; k += 4)
            {
                int blue = pixelBuffer[k];
                int green = pixelBuffer[k + 1];
                int red = pixelBuffer[k + 2];

                if (IsIPadSlateGray(blue, green, red))
                {
                    pixelBuffer[k] = 255;
                    pixelBuffer[k + 1] = 255;
                    pixelBuffer[k + 2] = 255;
                }
            }

            return WritePixelBuffer(sourceBitmap, pixelBuffer);
        }

        private static bool IsIPadSlateGray(int blue, int green, int red)
        {
            return IsBetweenRangeInclusive(blue, Tuple.Create(72, 79)) &&
                IsBetweenRangeInclusive(green, Tuple.Create(72, 79)) &&
                IsBetweenRangeInclusive(red, Tuple.Create(72, 79));
        }

        private static bool IsBetweenRangeInclusive(int focus, Tuple<int, int> range)
        {
            return range.Item1 <= focus && focus <= range.Item2;
        }

        private static Bitmap WritePixelBuffer(Bitmap sourceBitmap, byte[] pixelBuffer)
        {
            Bitmap resultBitmap = new Bitmap(sourceBitmap.Width, sourceBitmap.Height);

            BitmapData resultData = resultBitmap.LockBits(
                new Rectangle(0, 0, resultBitmap.Width, resultBitmap.Height),
                ImageLockMode.WriteOnly,
                PixelFormat.Format32bppArgb
            );

            Marshal.Copy(pixelBuffer, 0, resultData.Scan0, pixelBuffer.Length);
            resultBitmap.UnlockBits(resultData);

            return resultBitmap;
        }

        private static byte[] GetPixelBuffer(Bitmap sourceBitmap)
        {
            BitmapData sourceData = sourceBitmap.LockBits(
                new Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height),
                ImageLockMode.ReadOnly,
                PixelFormat.Format32bppArgb
            );

            byte[] pixelBuffer = new byte[sourceData.Stride * sourceData.Height];

            Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length);

            sourceBitmap.UnlockBits(sourceData);
            return pixelBuffer;
        }
    }
}
