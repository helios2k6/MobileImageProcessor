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
using System;
using System.Collections.Generic;

namespace Rip
{
    /// <summary>
    /// Utility class for grouping ImageJobs
    /// </summary>
    internal static class ImageJobGrouper
    {
        /// <summary>
        /// Groups ImageJobs by their timestamp 
        /// </summary>
        /// <param name="imageJobs">The ImageJobs object containing all of the individual ImageJob(s)</param>
        /// <returns>A map between timestamps and ImageJobGroups</returns>
        public static IDictionary<TimeSpan, ImageJobGroup> GroupImageJobs(ImageJobs imageJobs)
        {
            var groups = new Dictionary<TimeSpan, ImageJobGroup>();
            foreach (var imageJob in imageJobs.Images)
            {
                var focusTimeSpan = imageJob.SnapshotTimestamp;
                ImageJobGroup group;
                if (groups.TryGetValue(focusTimeSpan, out group) == false)
                {
                    group = new ImageJobGroup
                    {
                        Jobs = new HashSet<ImageJob>(),
                        Timestamp = focusTimeSpan,
                    };
                }

                group.Jobs.Add(imageJob);
            }

            return groups;
        }
    }
}
