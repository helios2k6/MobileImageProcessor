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
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace Slice
{
    /// <summary>
    /// Processes each image file by detecting the slice size and creating a 
    /// </summary>
    internal static class ImageProcessor
    {
        /// <summary>
        /// Process all images files
        /// </summary>
        /// <param name="files">The file paths you want to process</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of all of the processed images</returns>
        public static IEnumerable<ImageSliceContext> ProcessFiles(IEnumerable<string> files)
        {
            return files
                .Select(TryConvertRelativePathToAbsolutePath)
                .SelectWhereValueExist(t => ImageSliceContext.Create(t.ToMaybe()))
                .Select(TryLoadImage)
                .Select(TryCalculateSliceSize)
                .Select(TrySliceImage)
                .Select(TryWriteSliceImage)
                .Select(DisposeOfImageResources);
        }

        private static Maybe<string> TryConvertRelativePathToAbsolutePath(string file)
        {
            return Path.GetFullPath(file).ToMaybe();
        }

        private static ImageSliceContext TryWriteSliceImage(ImageSliceContext context)
        {
            var finalSliceImagePath = from sliceImage in context.SlicedImage
                                      from originalImagePath in context.OriginalFile
                                      from sliceImagePath in TryGetSlicedImagePath(originalImagePath)
                                      select TryWriteSliceImage(sliceImage, sliceImagePath);

            return ImageSliceContext.Create(
                context.OriginalFile,
                context.Image,
                context.SliceSize,
                context.SlicedImage,
                finalSliceImagePath
            );
        }

        private static Maybe<string> TryWriteSliceImage(Image image, string sliceImagePath)
        {
            try
            {
                image.Save(sliceImagePath);
                return sliceImagePath.ToMaybe();
            }
            catch (ArgumentException e)
            {
                Console.Error.WriteLine("Could not save image. An argument was null. {0}", e.Message);
            }
            catch (ExternalException e)
            {
                Console.Error.WriteLine("Could not save image. Incorrect format specified. {0}", e.Message);
            }

            return Maybe<string>.Nothing;
        }

        private static Maybe<string> TryGetSlicedImagePath(string originalFile)
        {
            var extension = Path.GetExtension(originalFile);
            var directoryPath = Path.GetDirectoryName(originalFile);
            var fileName = Path.GetFileNameWithoutExtension(originalFile);
            for (int i = 0; i < 10; i++)
            {
                var proposedSliceName = string.Format("{0}_slice_{1}{2}", fileName, i, extension);
                var proposedPath = Path.Combine(directoryPath, proposedSliceName);
                if (File.Exists(proposedPath) == false)
                {
                    return proposedPath.ToMaybe();
                }
            }

            Console.Error.WriteLine("Could not find available name for slice file for {0}", originalFile);
            return Maybe<string>.Nothing;
        }

        private static ImageSliceContext TrySliceImage(ImageSliceContext context)
        {
            var slicedImage = from originalFile in context.OriginalFile
                              from image in context.Image
                              from sliceSize in context.SliceSize
                              select ImageSlicer.TrySliceImage(originalFile, image, sliceSize);

            return ImageSliceContext.Create(
                context.OriginalFile,
                context.Image,
                context.SliceSize,
                slicedImage
            );
        }

        private static ImageSliceContext TryCalculateSliceSize(ImageSliceContext context)
        {
            return ImageSliceContext.Create(
                context.OriginalFile,
                context.Image,
                context.Image.Select(SliceCalculator.TryCalculateSliceDimensions)
            );
        }

        private static ImageSliceContext TryLoadImage(ImageSliceContext context)
        {
            return ImageSliceContext.Create(
                context.OriginalFile,
                context.OriginalFile.Select(CommonFunctions.TryLoadImage)
            );
        }

        private static ImageSliceContext DisposeOfImageResources(ImageSliceContext context)
        {
            using (context)
            {
                return ImageSliceContext.Create(
                    context.OriginalFile,
                    Maybe<Image>.Nothing,
                    context.SliceSize,
                    Maybe<Image>.Nothing,
                    context.SlicedImageFile
                );
            }
        }
    }
}
