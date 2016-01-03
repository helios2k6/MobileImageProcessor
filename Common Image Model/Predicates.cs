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

using System.Drawing;

namespace CommonImageModel
{
    /// <summary>
    /// Contains common predicates for determining what device the snapshot was taken with
    /// </summary>
    public static class Predicates
    {
        /// <summary>
        /// Determines if the image was taken with an iPad based on the image's dimensions
        /// </summary>
        /// <param name="image">The image</param>
        /// <returns>True if the image was taken from an iPad</returns>
        public static bool IsiPadSize(Image image)
        {
            return image.Height == 1536 && image.Width == 2048;
        }

        /// <summary>
        /// Determines if the image was taken with an iPhone 6 based on the image's dimensions
        /// </summary>
        /// <param name="image">The image</param>
        /// <returns>True if the image was taken from an iPhone 6</returns>
        public static bool IsiPhoneSixSize(Image image)
        {
            return image.Height == 750 && image.Width == 1334;
        }
    }
}
