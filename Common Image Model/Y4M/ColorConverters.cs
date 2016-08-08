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

using Functional.Maybe;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace CommonImageModel.Y4M
{
    /// <summary>
    /// A utility class for converting colors from different color planes
    /// </summary>
    public static class ColorConverters
    {
        #region public methods
        /// <summary>
        /// Try to convert from YCbCr to RGB colors
        /// </summary>
        /// <param name="colorSpace">The colorspace of the luma and chroma</param>
        /// <param name="luma">The luma plane</param>
        /// <param name="blueDifferential">The blue differential (Cb) plane</param>
        /// <param name="redDifferential">The red differential (Cr) plane</param>
        /// <param name="width">The width of the frame</param>
        /// <param name="height">The height of the frame</param>
        /// <returns>An RGB plane if this function succeeds. None otherwise</returns>
        /// <remarks>
        /// FFMPEG outputs 420mpeg2 (where the chroma samples are aligned horizontally, but 
        /// shifted a half-pixel down).
        /// https://msdn.microsoft.com/en-us/library/windows/desktop/dd206750(v=vs.85).aspx
        /// </remarks>
        public static Maybe<Color[][]> TryConvertYCbCr420ToRGB(
            ColorSpace colorSpace,
            byte[][] luma,
            byte[][] blueDifferential,
            byte[][] redDifferential,
            int width,
            int height
        )
        {
            return from upsampledBlueDifferential in TryUpsamplePlane(blueDifferential, width, height)
                   from upsampledRedDifferential in TryUpsamplePlane(redDifferential, width, height)
                   select TryConvertYCbCr444ToRGB(
                       luma,
                       upsampledBlueDifferential,
                       upsampledRedDifferential,
                       width,
                       height
                   );
        }
        #endregion

        #region private methods
        private static Maybe<Color[][]> TryConvertYCbCr444ToRGB(
            byte[][] luma,
            byte[][] upsampledBlueDifferential,
            byte[][] upsampledRedDifferential,
            int width,
            int height
        )
        {
            return Maybe<Color[][]>.Nothing;
        }

        /// <summary>
        /// Upsample a stream of bytes, interpreted as a chroma plane, using the 
        /// Centripetal Catmull-Rom spline method (cubic convolution interpolation). 
        /// This function will attempt to upsample, both, the horizontal and vertical
        /// </summary>
        /// <param name="frame">The bytes that represent the chroma plane</param>
        /// <returns>
        /// A new byte array that has been upsampled successfully, or none if the upsampling
        /// failed for any reason.
        /// </returns>
        private static Maybe<byte[][]> TryUpsamplePlane(
            byte[][] chromaPlane,
            int width,
            int height
        )
        {
            return from verticalUpsample in TryUpsample420To422(chromaPlane, width, height)
                   select TryUpsample422To444(verticalUpsample, width, height);
        }

        private static Maybe<byte[][]> TryUpsample420To422(
            byte[][] chromaPlane,
            int width,
            int height
        )
        {
            try
            {
                // UPSCALE THE VERTICAL LINES FIRST
            }
            catch (Exception)
            {
                return Maybe<byte[][]>.Nothing;
            }
            return Maybe<byte[][]>.Nothing;
        }

        private static byte GetUpsampledByte(
            IEnumerable<byte> chromaLine,
            int chromaPlaneIndex
        )
        {
            // Check to see if we're at the top edge
            if (chromaPlaneIndex == 0)
            {
                return Clamp((byte)((9 * (chromaLine.ElementAt(0) + chromaLine.ElementAt(1)) - (chromaLine.ElementAt(0) + chromaLine.ElementAt(2)) + 8) >> 4));
            }
            // Check to see if we're at the bottom edge
            else if (chromaPlaneIndex == chromaLine.Count() - 1)
            {
                return Clamp((byte)((9 * (chromaLine.ElementAt(chromaPlaneIndex) + chromaLine.ElementAt(chromaPlaneIndex)) - (chromaLine.ElementAt(chromaPlaneIndex - 1) + chromaLine.ElementAt(chromaPlaneIndex)) + 8) >> 4));
            }
            // We're somewhere in the middle, which is generalizable
            return Clamp((byte)((9 * (chromaLine.ElementAt(chromaPlaneIndex) + chromaLine.ElementAt(chromaPlaneIndex + 1)) - (chromaLine.ElementAt(chromaPlaneIndex - 1) + chromaLine.ElementAt(chromaPlaneIndex + 2)) + 8) >> 4));
        }

        private static Maybe<byte[][]> TryUpsample422To444(
            byte[][] upscaledVerticalChromaFrame,
            int width,
            int height
        )
        {
            try
            {
            }
            catch (Exception)
            {
                return Maybe<byte[][]>.Nothing;
            }
            return Maybe<byte[][]>.Nothing;
        }

        private static IEnumerable<T> Transpose<T>(T[][] jaggedArray)
        {
            IEnumerable<IEnumerator> enumerators = jaggedArray.Select(x => x.GetEnumerator());
            bool canMoveNext = enumerators.All(x => x.MoveNext());
            while (canMoveNext)
            {
                foreach (IEnumerator enumerator in enumerators)
                {
                    yield return (T)enumerator.Current;
                }

                canMoveNext = enumerators.All(x => x.MoveNext());
            }
        }

        /// <summary>
        /// Specific clamp function for limiting these values to the range of 1 byte
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        private static byte Clamp(byte val)
        {
            if (val.CompareTo(0) < 0) return 0;
            else if (val.CompareTo(255) > 0) return 255;
            else return val;
        }
        #endregion
    }
}
