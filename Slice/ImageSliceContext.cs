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

namespace Slice
{
    /// <summary>
    /// Represents the state of slicing an image
    /// </summary>
    public sealed class ImageSliceContext
    {
        /// <summary>
        /// The path to the original file
        /// </summary>
        public Maybe<string> OriginalFile { get; private set; }

        /// <summary>
        /// The image of the original file
        /// </summary>
        public Maybe<Image> Image { get; private set; }

        /// <summary>
        /// The size and position of the slice
        /// </summary>
        public Maybe<SliceSize> SliceSize { get; private set; }

        /// <summary>
        /// The sliced image
        /// </summary>
        public Maybe<Image> SlicedImage { get; private set; }

        /// <summary>
        /// The path to the sliced image file
        /// </summary>
        public Maybe<string> SlicedImageFile { get; private set; }

        /// <summary>
        /// Factory method for creating ImageSliceContextes
        /// </summary>
        /// <param name="originalFile">The original file path</param>
        /// <param name="image">The original image</param>
        /// <param name="sliceSize">The slice size for this image</param>
        /// <param name="sliceImage">The sliced image</param>
        /// <param name="slicedImageFile">The sliced image file path</param>
        /// <returns>A new ImageSliceContext</returns>
        public static ImageSliceContext Create(
            Maybe<string> originalFile,
            Maybe<Image> image,
            Maybe<SliceSize> sliceSize,
            Maybe<Image> sliceImage,
            Maybe<string> slicedImageFile
        )
        {
            return new ImageSliceContext
            {
                OriginalFile = originalFile,
                Image = image,
                SliceSize = sliceSize,
                SlicedImage = sliceImage,
                SlicedImageFile = slicedImageFile,
            };
        }

        /// <summary>
        /// Factory method for creating ImageSliceContextes
        /// </summary>
        /// <param name="originalFile">The original file path</param>
        /// <param name="image">The original image</param>
        /// <param name="sliceSize">The slice size for this image</param>
        /// <param name="sliceImage">The sliced image</param>
        /// <returns>A new ImageSliceContext</returns>
        public static ImageSliceContext Create(
            Maybe<string> originalFile,
            Maybe<Image> image,
            Maybe<SliceSize> sliceSize,
            Maybe<Image> sliceImage
        )
        {
            return new ImageSliceContext
            {
                OriginalFile = originalFile,
                Image = image,
                SliceSize = sliceSize,
                SlicedImage = sliceImage,
                SlicedImageFile = Maybe<string>.Nothing,
            };
        }

        /// <summary>
        /// Factory method for creating ImageSliceContextes
        /// </summary>
        /// <param name="originalFile">The original file path</param>
        /// <param name="image">The original image</param>
        /// <param name="sliceSize">The slice size for this image</param>
        /// <returns>A new ImageSliceContext</returns>
        public static ImageSliceContext Create(
            Maybe<string> originalFile,
            Maybe<Image> image,
            Maybe<SliceSize> sliceSize
        )
        {
            return new ImageSliceContext
            {
                OriginalFile = originalFile,
                Image = image,
                SliceSize = sliceSize,
                SlicedImage = Maybe<Image>.Nothing,
                SlicedImageFile = Maybe<string>.Nothing,
            };
        }

        /// <summary>
        /// Factory method for creating ImageSliceContextes
        /// </summary>
        /// <param name="originalFile">The original file path</param>
        /// <param name="image">The original image</param>
        /// <returns>A new ImageSliceContext</returns>
        public static ImageSliceContext Create(Maybe<string> originalFile, Maybe<Image> image)
        {
            return new ImageSliceContext
            {
                OriginalFile = originalFile,
                Image = image,
                SliceSize = Maybe<SliceSize>.Nothing,
                SlicedImage = Maybe<Image>.Nothing,
                SlicedImageFile = Maybe<string>.Nothing,
            };
        }

        /// <summary>
        /// Factory method for creating ImageSliceContextes
        /// </summary>
        /// <param name="originalFile">The original file path</param>
        /// <returns>A new ImageSliceContext</returns>
        public static ImageSliceContext Create(Maybe<string> originalFile)
        {
            return new ImageSliceContext
            {
                OriginalFile = originalFile,
                Image = Maybe<Image>.Nothing,
                SliceSize = Maybe<SliceSize>.Nothing,
                SlicedImage = Maybe<Image>.Nothing,
                SlicedImageFile = Maybe<string>.Nothing,
            };
        }
    }
}
