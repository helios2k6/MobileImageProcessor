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
using Functional.Maybe;
using System.Drawing;

namespace Match
{
    /// <summary>
    /// Class for holding utility functions for cropping images
    /// </summary>
    internal static class ImageCropper
    {
        private static readonly Point IPAD_VIEWPORT_ORIGIN = new Point(0, 190);
        private static readonly Point IPHONE_6_VIEWPORT_ORIGIN = new Point(0, 100);

        private static readonly Size IPAD_VIEWPORT_RECTANGLE = new Size(2048, 1150);
        private static readonly Size IPHONE_6_VIEWPORT_RECTANGLE = new Size(1334, 550);

        /// <summary>
        /// Try to crop a snapshot taken from an iPad
        /// </summary>
        /// <param name="originalImage">The original snapshot from the iPad</param>
        /// <returns>A new image with the top scrubber and bottom controls cropped out</returns>
        public static Maybe<ImageWrapper> TryCropiPadImage(ImageWrapper originalImage)
        {
            return from croppedImage in ImageTransformations.TryCropImage(
                       originalImage.Image,
                       IPAD_VIEWPORT_ORIGIN,
                       IPAD_VIEWPORT_RECTANGLE
                   )
                   select new ImageWrapper(croppedImage, originalImage.ImagePath);
        }

        /// <summary>
        /// Try to crop a snapshot taken from an iPhone 6
        /// </summary>
        /// <param name="originalImage">The original snapshot from the iPhone 6</param>
        /// <returns>A new image with the top scrubber and bottom controls cropped out</returns>
        public static Maybe<ImageWrapper> TryCropiPhoneSixImage(ImageWrapper originalImage)
        {
            return from croppedImage in ImageTransformations.TryCropImage(
                       originalImage.Image,
                       IPHONE_6_VIEWPORT_ORIGIN,
                       IPHONE_6_VIEWPORT_RECTANGLE
                   )
                   select new ImageWrapper(croppedImage, originalImage.ImagePath);
        }
    }
}
