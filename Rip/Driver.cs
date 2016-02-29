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
using System.IO;
using System.Linq;

namespace Rip
{
    /// <summary>
    /// Main entry point to the Rip process
    /// </summary>
    internal static class Driver
    {
        public static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                PrintHelp();
                return;
            }

            var imageJobs = CommonFunctions.TryReadStandardIn();
            if (imageJobs.IsNothing())
            {
                PrintHelp();
                return;
            }
            using (new TimingToken("Rip", true))
            {
                var newImageJobs = SnapshotHarvester.HarvestCandidateImages(
                    imageJobs.Value,
                    GetAllMediaFiles(args)
                );

                Console.WriteLine(JsonConvert.SerializeObject(newImageJobs));
            }

            CommonFunctions.CloseAllStandardFileHandles();
        }

        private static IEnumerable<string> GetAllMediaFiles(IEnumerable<string> folderPaths)
        {
            return folderPaths.SelectMany(GetAllMediaFiles);
        }

        private static IEnumerable<string> GetAllMediaFiles(string folderPath)
        {
            if (Directory.Exists(folderPath))
            {
                var listOfFileLists = new[]
                {
                    Directory.EnumerateFiles(folderPath, "*.mkv", SearchOption.AllDirectories),
                    Directory.EnumerateFiles(folderPath, "*.mp4", SearchOption.AllDirectories),
                    Directory.EnumerateFiles(folderPath, "*.wmv", SearchOption.AllDirectories),
                };
                return listOfFileLists.SelectMany(x => x);
            }
            else
            {
                Console.Error.WriteLine("Folder {0} does not exist", folderPath);
            }
            return Enumerable.Empty<string>();
        }

        private static void PrintHelp()
        {
            Console.Error.WriteLine("Rip 1.0");
            Console.Error.WriteLine("Usage: <this app> <list of folders with video files>");
        }
    }
}
