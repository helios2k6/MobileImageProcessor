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
using System.Drawing;
using System;

namespace CommonImageModel.Y4M
{
    /// <summary>
    /// A utility class for converting colors from different color planes
    /// </summary>
    public static class ColorConverters
    {
        #region private fields
        #endregion

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
        public static Maybe<Color[][]> TryConvertToRGB(
            ColorSpace colorSpace,
            byte[] luma,
            byte[] blueDifferential,
            byte[] redDifferential,
            int width,
            int height
        )
        {
            // TODO: Finish this
            return Maybe<Color[][]>.Nothing;
        }
        #endregion

        #region private methods
        private static Maybe<Color[][]> ConvertYCbCr444ToRGB888(
            byte[] luma,
            byte[] blueDifferential,
            byte[] redDifferential,
            int width,
            int height
        )
        {
            return Maybe<Color[][]>.Nothing;
        }

        private static Maybe<Color[][]> ConvertYCbCr422ToYCbCr444(
            byte[] luma,
            byte[] blueDifferential,
            byte[] redDifferential,
            int width,
            int height
        )
        {
            return Maybe<Color[][]>.Nothing;
        }

        private static Maybe<Color[][]> ConvertYCbCr420ToYCbCr422(
            byte[] luma,
            byte[] blueDifferential,
            byte[] redDifferential,
            int width,
            int height
        )
        {
            return Maybe<Color[][]>.Nothing;
        }

        #endregion
    }
}
