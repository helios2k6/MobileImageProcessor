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
    /// Factory method for slicing images
    /// </summary>
    public static class ImageSlicer
    {
        /// <summary>
        /// Attempts to slice the image
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static Maybe<Image> TrySliceImage(
            string imagePath,
            Image image,
            SliceSize sliceSize
        )
        {
            var bitmap = new Bitmap(image);
            var cropArea = new Rectangle(
                new Point(sliceSize.XOffset, sliceSize.YOffset),
                new Size(sliceSize.Width, sliceSize.Height)
            );

            try
            {
                return from tintedImage in TryInvertColors(bitmap.Clone(cropArea, bitmap.PixelFormat))
                       select tintedImage as Image;
            }
            catch (OutOfMemoryException e)
            {
                Console.Error.WriteLine("Could not slice image. Crop area is outside of the image bounds. {0}", e.Message);
            }
            catch (ArgumentException e)
            {
                Console.Error.WriteLine("Could not slice image. Invalid arguments. {0}", e.Message);
            }

            return Maybe<Image>.Nothing;
        }

        private static Maybe<Bitmap> TryInvertColors(Bitmap sliceImage)
        {
            try
            {
                return sliceImage.FilterIPadSnapshot().ToMaybe();
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Could not add color tint to image. {0}", e.Message);
            }

            return Maybe<Bitmap>.Nothing;
        }
    }
}
