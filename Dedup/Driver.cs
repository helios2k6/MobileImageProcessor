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
using System.Collections.Generic;
using System.Linq;

namespace Dedup
{
    /// <summary>
    /// The main entry point for the Dedup process
    /// </summary>
    internal static class Driver
    {
        /// <summary>
        /// Main method that serves as the entry point for the Dedup process
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            var imageJobsMaybe = CommonFunctions.TryReadStandardIn();
            if (imageJobsMaybe.IsNothing())
            {
                PrintHelp();
                return;
            }

            using (new TimingToken("Dedup", true))
            {
                IEnumerable<SnapshotContext> loadedSnapshots = SnapshotLoader.LoadSnapshots(imageJobsMaybe.Value);
                IEnumerable<IEnumerable<SnapshotContext>> duplicateSnapshotGroups = DuplicateSnapshotDetector.DetectDuplicates(loadedSnapshots);
                IEnumerable<SnapshotContext> remainingSnapshots = DuplicateSnapshotProcessor.DeleteDuplicateImages(duplicateSnapshotGroups);
                IEnumerable<string> pathToRemainingSnapshots = remainingSnapshots.Select(s => s.SnapshotPath);

                DisposeOfOldSnapshots(loadedSnapshots);
                loadedSnapshots = null;
                remainingSnapshots = null;
                duplicateSnapshotGroups = null;

                ImageJobs newImageJobs = DeletedSnapshotsCoalescer.CoalesceDeletedSnapshots(
                    imageJobsMaybe.Value,
                    pathToRemainingSnapshots
                );
                Console.WriteLine(JsonConvert.SerializeObject(newImageJobs));
            }
            CommonFunctions.CloseAllStandardFileHandles();
        }

        private static void DisposeOfOldSnapshots(IEnumerable<SnapshotContext> snapshots)
        {
            foreach (var s in snapshots)
            {
                s.Dispose();
            }
        }

        private static void PrintHelp()
        {
            Console.Error.WriteLine("Dedup 1.0 - Removes all duplicate anime snapshots");
            Console.Error.WriteLine("Usage: This program does not take any args and requires all input be from stdin");
        }
    }
}
