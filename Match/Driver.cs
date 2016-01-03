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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Match
{
    /// <summary>
    /// The main entry point for the Match process
    /// </summary>
    internal static class Driver
    {
        /// <summary>
        /// The main function that executes for the Match process
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            Maybe<ImageJobs> imageJobsMaybe = CommonFunctions.TryReadStandardIn();
            if (imageJobsMaybe.IsNothing())
            {
                PrintHelp();
                return;
            }

            IDictionary<ImageJob, string> jobToSnapshotMap = ImageMatcher.GetMatches(imageJobsMaybe.Value);
            ImageJob[] processedJobs = UpdateSnapshotResults(jobToSnapshotMap).ToArray();
            var processedImageJobs = new ImageJobs
            {
                Images = processedJobs,
            };

            Console.WriteLine(JsonConvert.SerializeObject(processedImageJobs));
        }

        private static IEnumerable<ImageJob> UpdateSnapshotResults(IDictionary<ImageJob, string> jobToSnapshotMap)
        {
            foreach (var jobToSnapshot in jobToSnapshotMap)
            {
                ImageJob previousImageJob = jobToSnapshot.Key;
                string snapshot = jobToSnapshot.Value;
                yield return new ImageJob
                {
                    OriginalFilePath = previousImageJob.OriginalFilePath,
                    SliceImagePath = previousImageJob.SliceImagePath,
                    SnapshotTimestamp = previousImageJob.SnapshotTimestamp,
                    ImageSnapshots = new[] { snapshot },
                };
            }
        }

        private static void PrintHelp()
        {
            Console.Error.WriteLine("Match 1.0 - Detects the most likely snapshot match");
            Console.Error.WriteLine("Usage: This program does not take any args and requires all input be from stdin");
        }
    }
}
