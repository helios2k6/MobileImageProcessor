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
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Dispatch
{
    /// <summary>
    /// Dispatches the processes of the Media Snapshot Processor Framework
    /// </summary>
    internal static class Dispatcher
    {
        private static ICollection<string> SCRAPE = new HashSet<string>
        {
            "Scrape",
            "Scrape.exe",
            "scrape",
            "scrape.exe",
        };

        private static ICollection<string> DEDUP = new HashSet<string>
        {
            "Dedup",
            "Dedup.exe",
            "dedup",
            "dedup.exe",
        };

        private static ICollection<string> MATCH = new HashSet<string>
        {
            "Match",
            "Match.exe",
            "match",
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
        /// <returns>
        /// A Task representing this operation
        /// </returns>
        public async static Task DispatchAllProcessesAsync(
            string folderOfSnapshots,
            string folderOfMediaFiles
        )
        {
            foreach (var snapshot in GetSnapshots(folderOfSnapshots))
            {
                await CyclePipeLineAsync(snapshot, folderOfSnapshots);
            }
        }

        private static IEnumerable<string> GetSnapshots(string pathToFolder)
        {
            return Directory.EnumerateFiles(
                pathToFolder,
                "*.png",
                SearchOption.AllDirectories
            );
        }

        private async static Task CyclePipeLineAsync(string snapshot, string folderOfMediaFiles)
        {
            var slice = new SliceProcess(snapshot);
            var scrape = new GeneralProcess(SCRAPE);
            var rip = new RipProcess(folderOfMediaFiles);
            var dedup = new GeneralProcess(DEDUP);
            var match = new GeneralProcess(MATCH);

            var sliceOutput = await slice.ExecuteAsync(Maybe<string>.Nothing);
            var scrapeOutput = await scrape.ExecuteAsync(sliceOutput.ToMaybe());
            var ripOutput = await rip.ExecuteAsync(scrapeOutput.ToMaybe());
            var dedupOutput = await dedup.ExecuteAsync(ripOutput.ToMaybe());
            await match.ExecuteAsync(dedupOutput.ToMaybe());
        }
    }
}
