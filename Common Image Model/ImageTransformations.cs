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
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace CommonImageModel
{
    /// <summary>
    /// A utility class for performing transformations on an Image object
    /// </summary>
    public static class ImageTransformations
    {
        /// <summary>
        /// Attempts to blur the image by the specified amount.
        /// </summary>
        public static Maybe<Image> TryBlurImage(Image image, int radius)
        {
            return new FastAccBoxBlurGaussianBlurTransformation(image, radius).TryTransform();
        }

        /// <summary>
        /// An exception safe version of <seealso cref="ResizeImage"/>
        /// </summary>
        /// <param name="image">The Image to resize</param>
        /// <param name="width">The desired resized image's width</param>
        /// <param name="height">The desired resized image's height</param>
        /// <returns>A new Image resized to the specified width and height</returns>
        public static Maybe<Image> TryResizeImage(Image image, int width, int height)
        {
            return new ResizeTransformation(image, width, height).TryTransform();
        }

        /// <summary>
        /// An exception safe version of <seealso cref="CropImage"/>
        /// </summary>
        /// <param name="image">The Image to crop</param>
        /// <param name="point">The upper left-hand corner of the viewport rectangle</param>
        /// <param name="size">The rectangle width and height to crop</param>
        /// <returns>A newly cropped image</returns>
        public static Maybe<Image> TryCropImage(Image image, Point point, Size size)
        {
            return new CropTransformation(image, point, size).TryTransform();
        }

        private static Maybe<T> WrapFuncInMaybe<T>(Func<T> func)
        {
            try
            {
                return func.Invoke().ToMaybe<T>();
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
            }

            return Maybe<T>.Nothing;
        }
    }
}
