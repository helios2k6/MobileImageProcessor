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
using System.IO;
using System.Linq;

namespace Rip
{
    /// <summary>
    /// Harvests all possible images for a given ImageJob
    /// </summary>
    internal static class SnapshotHarvester
    {
        /// <summary>
        /// Harvest all of the possible images for a given ImageJob
        /// </summary>
        /// <param name="imageJobs">The old ImageJobs object</param>
        /// <param name="mediaFilePaths">The path to every single media file</param>
        /// <returns>An new ImageJobs object with paths to the snapshots for every ImageJob</returns>
        public static ImageJobs HarvestCandidateImages(
            ImageJobs imageJobs,
            IEnumerable<string> mediaFilePaths
        )
        {
            IDictionary<TimeSpan, ImageJobGroup> imageGroups = ImageJobGrouper.GroupImageJobs(imageJobs);
            var mapFromGroupsToSnapshots = new Dictionary<ImageJobGroup, IEnumerable<string>>();
            foreach (var group in imageGroups.Values)
            {
                var snapshots = TakeSnapshots(group, mediaFilePaths);
                mapFromGroupsToSnapshots.Add(group, snapshots);
            }

            return new ImageJobs
            {
                Images = UpdateImageJobs(mapFromGroupsToSnapshots).ToArray(),
            };
        }

        private static IEnumerable<ImageJob> UpdateImageJobs(
            IDictionary<ImageJobGroup, IEnumerable<string>> mapFromGroupsToSnapshots
        )
        {
            foreach (var groupToSnapshots in mapFromGroupsToSnapshots)
            {
                string[] snapshots = groupToSnapshots.Value.ToArray();
                ImageJobGroup group = groupToSnapshots.Key;
                foreach (var imageJob in group.Jobs)
                {
                    var newImageJob = new ImageJob
                    {
                        OriginalFilePath = imageJob.OriginalFilePath,
                        SliceImagePath = imageJob.SliceImagePath,
                        SnapshotTimestamp = imageJob.SnapshotTimestamp,
                        ImageSnapshots = snapshots,
                    };

                    yield return newImageJob;
                }
            }
        }

        private static IEnumerable<string> TakeSnapshots(ImageJobGroup group, IEnumerable<string> mediaFilePaths)
        {
            var listOfFilePathLists = new List<IEnumerable<string>>();
            foreach (var mediaFilePath in mediaFilePaths)
            {
                using (var ffmpeg = new FFMPEGProcess(group.Timestamp, mediaFilePath))
                {
                    ffmpeg.Execute();
                }

                listOfFilePathLists.Add(GetImageFilePaths(group, mediaFilePath));
            }

            return listOfFilePathLists.SelectMany(s => s);
        }

        private static IEnumerable<string> GetImageFilePaths(ImageJobGroup group, string mediaFilePath)
        {
            var rootMediaFileName = Path.GetFileNameWithoutExtension(mediaFilePath);
            return Directory.EnumerateFiles(
                Directory.GetCurrentDirectory(),
                string.Format(
                    "AUTOGEN_({0})_TIME_({1})_SNAPSHOT*.png",
                    rootMediaFileName,
                    FFMPEGProcess.FormatTimeSpanFileName(group.Timestamp)
                )
            );
        }
    }
}