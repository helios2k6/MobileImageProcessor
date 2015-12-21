using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

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
