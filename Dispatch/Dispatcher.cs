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
using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Dispatch
{
    /// <summary>
    /// Dispatches the processes of the Media Snapshot Processor Framework
    /// </summary>
    internal static class Dispatcher
    {
        private static ICollection<string> SLICE = new HashSet<string>
        {
            "Slice.exe",
            "slice.exe",
        };

        private static ICollection<string> RIP = new HashSet<string>
        {
            "Rip.exe",
            "rip.exe",
        };

        private static ICollection<string> SHIFT = new HashSet<string>
        {
            "Shift.exe",
            "shift.exe",
        };

        private static ICollection<string> SCRAPE = new HashSet<string>
        {
            "Scrape.exe",
            "scrape.exe",
        };

        private static ICollection<string> DEDUP = new HashSet<string>
        {
            "Dedup.exe",
            "dedup.exe",
        };

        private static ICollection<string> MATCH = new HashSet<string>
        {
            "Match.exe",
            "match.exe",
        };

        /// <summary>
        /// Dispatch the Processes necessary to process all of the snapshots without
        /// exhausting all resources
        /// </summary>
        /// <param name="folderOfSnapshots">
        /// Path to the folder of snapshots
        /// </param>
        /// <param name="folderOfMediaFiles">
        /// Path to the folder of media files
        /// </param>
        /// <param name="timeShift">
        /// The amount of time to shift the timecodes by
        /// </param>
        /// <returns>
        /// A Task representing this operation and the results of this Dispatcher run
        /// </returns>
        public async static Task<DispatcherResults> DispatchAllProcessesAsync(
            string folderOfSnapshots,
            string folderOfMediaFiles,
            int? timeShift
        )
        {
            Maybe<Task<DispatcherResults>> dispatchAttempt = from snapshots in TryGetSnapshots(folderOfSnapshots)
                                                             select FireProcessesAsync(folderOfMediaFiles, timeShift, snapshots);

            if (dispatchAttempt.IsNothing())
            {
                return new DispatcherResults(Enumerable.Empty<ImageJob>(), Enumerable.Empty<ImageJob>());
            }

            return await dispatchAttempt.Value;
        }

        private async static Task<DispatcherResults> FireProcessesAsync(
            string folderOfMediaFiles,
            int? timeShift,
            string sliceProgramArgs
        )
        {
            using (var slice = new GeneralProcess(SLICE, sliceProgramArgs))
            {
                using (var scrape = new GeneralProcess(SCRAPE))
                {
                    string sliceOutput = await slice.ExecuteAsync(Maybe<string>.Nothing);
                    string scrapeOutput = await scrape.ExecuteAsync(sliceOutput.ToMaybe());
                    string shiftOutput = await MaybeRunTimeShiftAsync(scrapeOutput, timeShift);
                    return await RunImageRippers(shiftOutput, folderOfMediaFiles);
                }
            }
        }

        private async static Task<string> MaybeRunTimeShiftAsync(string scrapeOutput, int? timeShift)
        {
            if (timeShift == null)
            {
                return scrapeOutput;
            }

            using (var shift = new GeneralProcess(SCRAPE, timeShift.ToString()))
            {
                return await shift.ExecuteAsync(scrapeOutput.ToMaybe());
            }
        }

        private async static Task<DispatcherResults> RunImageRippers(
            string scrapeOutput,
            string folderOfMediaFiles
        )
        {
            ImageJobs deserializedModel = JsonConvert.DeserializeObject<ImageJobs>(scrapeOutput);
            var unsuccessfulImageJobs = new List<ImageJob>();
            var sucessfulImageJobs = new List<ImageJob>();
            foreach (ImageJob imageJob in deserializedModel.Images)
            {
                try
                {
                    string matchOutput = await ProcessImageJobAsync(imageJob, folderOfMediaFiles);
                    ImageJobs processedImageJobs = JsonConvert.DeserializeObject<ImageJobs>(matchOutput);
                    ImageJob processedImageJob = processedImageJobs.Images.First();
                    if (processedImageJob.ImageSnapshots.Any())
                    {
                        sucessfulImageJobs.Add(imageJob);
                    }
                    else
                    {
                        unsuccessfulImageJobs.Add(imageJob);
                    }

                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(
                        "Unable to process snapshot {0}. Exception: {1}",
                        imageJob.OriginalFilePath,
                        e.Message
                    );
                    unsuccessfulImageJobs.Add(imageJob);
                }
            }

            return new DispatcherResults(sucessfulImageJobs, unsuccessfulImageJobs);
        }

        private async static Task<string> ProcessImageJobAsync(ImageJob imageJob, string folderOfMediaFiles)
        {
            var newImageJobs = new ImageJobs
            {
                Images = new[] { imageJob },
            };

            var serializedNewImageJobs = JsonConvert.SerializeObject(newImageJobs);

            using (var rip = new GeneralProcess(RIP, string.Format("\"{0}\"", folderOfMediaFiles)))
            {
                using (var dedup = new GeneralProcess(DEDUP))
                {
                    using (var match = new GeneralProcess(MATCH))
                    {
                        string ripOutput = await rip.ExecuteAsync(serializedNewImageJobs.ToMaybe());
                        string dedupOutput = await dedup.ExecuteAsync(ripOutput.ToMaybe());
                        string matchOutput = await match.ExecuteAsync(dedupOutput.ToMaybe());
                        MoveCompletedPhotos(matchOutput);
                        return matchOutput;
                    }
                }
            }
        }

        private static void MoveCompletedPhotos(string matchOutput)
        {
            if (Directory.Exists("Finished") == false)
            {
                Directory.CreateDirectory("Finished");
            }

            var finalImageModel = JsonConvert.DeserializeObject<ImageJobs>(matchOutput);
            foreach (var imageJob in finalImageModel.Images)
            {
                foreach (var snapshot in imageJob.ImageSnapshots)
                {
                    var fileName = Path.GetFileName(snapshot);
                    var destPath = Path.Combine("Finished", fileName);
                    try
                    {
                        File.Move(snapshot, destPath);
                    }
                    catch (IOException e)
                    {
                        Console.Error.WriteLine("File {0} already exists. Exception: {1}", destPath, e);
                    }

                }
            }
        }

        private static Maybe<string> TryGetSnapshots(string pathToFolder)
        {
            return Directory.EnumerateFiles(pathToFolder, "*.png", SearchOption.TopDirectoryOnly)
                .Concat(Directory.EnumerateFiles(pathToFolder, "*.PNG", SearchOption.TopDirectoryOnly))
                .Select(Path.GetFullPath)
                .Distinct()
                .Select(s => string.Format("\"{0}\"", s)) // Quote the paths
                .IfNotEmpty(
                    e => e.Aggregate<string>((a, b) => string.Format("{0} {1}", a, b)).Trim(),
                    null
                ).ToMaybe();
        }
    }
}
