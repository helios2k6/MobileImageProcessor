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
using System.Drawing;

namespace Slice
{
    /// <summary>
    /// Calculates the size of the slice that is required for this particular Image
    /// </summary>
    internal static class SliceCalculator
    {
        /// <summary>
        /// Calculate the size of the slice that is required 
        /// </summary>
        /// <param name="image">The original image</param>
        /// <returns>
        /// A new <see cref="SliceSize"/> that describes how to slice
        /// the photo
        /// </returns>
        public static Maybe<SliceSize> TryCalculateSliceDimensions(Image image)
        {
            if (IsiPadSize(image))
            {
                return SliceSize.IPadSliceSize.ToMaybe();
            }

            if (IsiPhoneSize(image))
            {
                return SliceSize.IPhoneSliceSize.ToMaybe();
            }

            return Maybe<SliceSize>.Nothing;
        }

        private static bool IsiPadSize(Image image)
        {
            return image.Height == 1536 &&
                image.Width == 2048;
        }

        private static bool IsiPhoneSize(Image image)
        {
            return image.Height == 750 &&
                image.Width == 1334;
        }
    }
}
