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
using System.Linq;

namespace Dedup
{
    /// <summary>
    /// Loads all of the images from the ImageJobs and resizes the images so that they'll fit in memory
    /// </summary>
    internal static class SnapshotLoader
    {
        private const int TARGET_WIDTH = 640;
        private const int TARGET_HEIGHT = 360;

        /// <summary>
        /// Load all of the images from disk and resize them
        /// </summary>
        /// <param name="imageJobs"></param>
        /// <returns></returns>
        public static IEnumerable<SnapshotContext> LoadSnapshots(ImageJobs imageJobs)
        {
            return GetAllImagePaths(imageJobs)
                .Select(TryLoadSnapshot)
                .SelectWhereValueExist(i => i);
        }

        private static Maybe<SnapshotContext> TryLoadSnapshot(string snapshotPath)
        {
            return from image in CommonFunctions.TryLoadImage(snapshotPath)
                   from transformedImage in ImageTransformations.TryResizeImage(image, TARGET_WIDTH, TARGET_WIDTH)
                   let lockbitImage = new LockBitImage(transformedImage)
                   select CommonFunctions.ExecThenDispose(
                        () => new SnapshotContext(snapshotPath, lockbitImage, GetFingerPrint(lockbitImage)),
                        image,
                        transformedImage
                   );
        }

        private static int GetFingerPrint(LockBitImage image)
        {
            // Remember: x,y is in the top left corner
            Color topLeft = image.GetPixel(0, 0);
            Color topRight = image.GetPixel(TARGET_WIDTH - 1, 0);
            Color bottomLeft = image.GetPixel(0, TARGET_HEIGHT - 1);
            Color bottomRight = image.GetPixel(TARGET_WIDTH - 1, TARGET_HEIGHT - 1);

            return GetColorHashOut(topLeft) +
                GetColorHashOut(topRight) +
                GetColorHashOut(bottomLeft) +
                GetColorHashOut(bottomRight);
        }

        private static int GetColorHashOut(Color color)
        {
            return color.R + color.G + color.B;
        }

        private static IEnumerable<string> GetAllImagePaths(ImageJobs imageJobs)
        {
            return new HashSet<string>(imageJobs.Images.SelectMany(s => s.ImageSnapshots));
        }
    }
}
