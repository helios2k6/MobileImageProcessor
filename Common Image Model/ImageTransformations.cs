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
        private const int PRECISION = 1000;

        /// <summary>
        /// Resize an image to an arbitrary width and height
        /// </summary>
        /// <remarks>
        /// This function always returns a new Image object, even if no resizing takes place
        /// </remarks>
        /// <param name="image">The Image to resize</param>
        /// <param name="width">The desired resized image's width</param>
        /// <param name="height">The desired resized image's height</param>
        /// <returns>A new Image resized to the specified width and height</returns>
        public static Image ResizeImage(Image image, int width, int height)
        {
            // Easy check to avoid lots of work for things already sized properly
            if (width == image.Width && height == image.Height)
            {
                return image.Clone() as Image;
            }

            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        /// <summary>
        /// Attempts to scale an image down by a specific coefficient, preserving the aspect ratio
        /// </summary>
        /// <remarks>
        /// This function always returns a new Image object, even if no scaling takes place
        /// </remarks>
        /// <param name="image">The image to scale</param>
        /// <param name="scale">The amount to scale the image by</param>
        /// <returns>The newly scaled image</returns>
        public static Image ScaleImage(Image image, double scale)
        {
            if (double.IsInfinity(scale) || double.IsNaN(scale) || scale < 0)
            {
                throw new ArgumentException("Scale cannot be infinity, NaN, or less than 0");
            }

            int scaleAsInt = (int)Math.Floor(scale * PRECISION);

            // Scale was 1.0. No need to transform anything
            if (scaleAsInt - PRECISION == 0)
            {
                return image.Clone() as Image;
            }

            int newWidth = (int)Math.Floor(((double)(scaleAsInt * image.Width)) / PRECISION);
            int newHeight = (int)Math.Floor(((double)(scaleAsInt * image.Height)) / PRECISION);

            return ResizeImage(image, newWidth, newHeight);
        }

        /// <summary>
        /// Crop an Image given the upper left-hand corner of the viewport
        /// and the width and height of the viewport rectangle
        /// </summary>
        /// <remarks>
        /// This function always returns a new Image, even if no cropping takes place
        /// </remarks>
        /// <param name="image">The Image to crop</param>
        /// <param name="point">The upper left-hand corner of the viewport rectangle</param>
        /// <param name="size">The rectangle width and height to crop</param>
        /// <returns>A newly cropped image</returns>
        public static Image CropImage(Image image, Point point, Size size)
        {
            if (point.X == 0 && point.Y == 0 && size.Width == image.Width && size.Height == image.Height)
            {
                return image.Clone() as Image;
            }

            using (var bitmap = new Bitmap(image))
            {
                return (bitmap.Clone(new Rectangle(point, size), bitmap.PixelFormat) as Image);
            }
        }

        /// <summary>
        /// An exception safe version of <seealso cref="ScaleImage"/>
        /// </summary>
        /// <param name="image">The image to scale</param>
        /// <param name="scale">The amount to scale the image by</param>
        /// <returns>The newly scaled image</returns>
        public static Maybe<Image> TryScaleImage(Image image, double scale)
        {
            return WrapFuncInMaybe<Image>(() => ScaleImage(image, scale));
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
            return WrapFuncInMaybe<Image>(() => ResizeImage(image, width, height));
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
            return WrapFuncInMaybe<Image>(() => CropImage(image, point, size));
        }

        private static Maybe<T> WrapFuncInMaybe<T>(Func<T> func)
        {
            try
            {
                func.Invoke().ToMaybe<T>();
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
            }

            return Maybe<T>.Nothing;
        }
    }
}
