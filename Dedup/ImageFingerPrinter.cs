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

using CommonImageModel;
using System;
using System.Drawing;
using System.Linq;

namespace Dedup
{
    /// <summary>
    /// Class that fingerprints an image
    /// </summary>
    internal static class ImageFingerPrinter
    {
        /// <summary>
        /// Calculate the fingerprint of an image
        /// </summary>
        /// <param name="image">The image to fingerprint</param>
        /// <returns>An int representing this image's fingerprint</returns>
        public static int CalculateFingerPrint(LockBitImage image)
        {
            return -1;
        }

        /// <summary>
        /// Gets the fingerprint of a square of pixels 
        /// </summary>
        /// <param name="image">The image</param>
        /// <param name="x">The x coordinate of the top left corner of the square</param>
        /// <param name="y">The y coordinate of the top left corner of the square</param>
        /// <returns>The fingerprint of this specific square</returns>
        private static int GetSquareFingerPrint(LockBitImage image, int x, int y, int length)
        {
            return (int)Math.Floor((from xCoord in Enumerable.Range(x, length)
                                    from yCoord in Enumerable.Range(y, length)
                                    select image.GetPixel(xCoord, yCoord)).Average<Color>(c => CalculateColorHash(c)));
        }

        private static int CalculateColorHash(Color color)
        {
            return color.R + color.G + color.B;
        }
    }
}
