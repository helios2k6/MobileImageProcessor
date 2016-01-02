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
using System.Collections.Generic;
using System.Linq;

namespace Dedup
{
    /// <summary>
    /// Coalsces deleted snapshot paths from the old ImageJobs object
    /// </summary>
    internal static class DeletedSnapshotsCoalescer
    {
        /// <summary>
        /// Culls the deleted snapshots from ImageJobs and produces a new ImageJobs object
        /// with the remaining snapshots
        /// </summary>
        /// <param name="oldImageJobs">The old ImageJobs object</param>
        /// <param name="remainingSnapshots">The path to all of the remaining snapshots</param>
        /// <returns>A new ImageJobs object</returns>
        public static ImageJobs CoalesceDeletedSnapshots(
            ImageJobs oldImageJobs,
            IEnumerable<string> remainingSnapshots
        )
        {
            var setOfRemainingSnapshots = new HashSet<string>(remainingSnapshots);
            var newImageJobs = oldImageJobs.Images.Select(s => ProcessImageJob(s, setOfRemainingSnapshots));
            return new ImageJobs
            {
                Images = newImageJobs.ToArray(),
            };
        }

        private static ImageJob ProcessImageJob(
            ImageJob oldImageJob,
            ICollection<string> remainingSnapshotPaths
        )
        {
            var newSnapshotPaths = new List<string>();
            foreach (var oldSnapshotPath in oldImageJob.ImageSnapshots)
            {
                if (remainingSnapshotPaths.Contains(oldSnapshotPath))
                {
                    newSnapshotPaths.Add(oldSnapshotPath);
                }
            }

            return new ImageJob
            {
                OriginalFilePath = oldImageJob.OriginalFilePath,
                SliceImagePath = oldImageJob.SliceImagePath,
                SnapshotTimestamp = oldImageJob.SnapshotTimestamp,
                ImageSnapshots = newSnapshotPaths.ToArray(),
            };
        }
    }
}
