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
        /// <summary>
        /// Load all of the images from disk and resize them
        /// </summary>
        /// <param name="imageJobs"></param>
        /// <returns></returns>
        public static IEnumerable<SnapshotContext> LoadSnapshots(ImageJobs imageJobs)
        {
            return GetAllImagePaths(imageJobs)
                    .Select(LoadFullSizedSnapshots)
                    .SelectWhereValueExist(ResizeImageAndDisposeOfOriginal);
        }

        private static SnapshotContext ResizeImageAndDisposeOfOriginal(Tuple<string, Image> snapshotPathAndImage)
        {
            Image originalSnapshot = snapshotPathAndImage.Item2;
            using (originalSnapshot)
            {
                return new SnapshotContext(
                    snapshotPathAndImage.Item1,
                    ImageResizer.ResizeImageDown(originalSnapshot).ToMaybe()
                );
            }
        }

        private static Maybe<Tuple<string, Image>> LoadFullSizedSnapshots(string snapshotPath)
        {
            return from image in CommonFunctions.TryLoadImage(snapshotPath)
                   select Tuple.Create(snapshotPath, image);
        }

        private static IEnumerable<string> GetAllImagePaths(ImageJobs imageJobs)
        {
            return new HashSet<string>(imageJobs.Images.SelectMany(s => s.ImageSnapshots));
        }
    }
}
