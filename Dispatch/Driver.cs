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

namespace Dispatch
{
    /// <summary>
    /// The main entry point for the Dispatch process
    /// </summary>
    internal static class Driver
    {
        /// <summary>
        /// The main function that executes the Dispatch process
        /// </summary>
        /// <param name="args">Program arguments</param>
        public static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                PrintHelp();
                return;
            }

            var folderOfPictures = args[0];
            var folderOfVideos = args[1];

            if (ValidateFolder(folderOfPictures) == false || ValidateFolder(folderOfVideos) == false)
            {
                return;
            }

            DispatcherResults results = Dispatcher.DispatchAllProcessesAsync(folderOfPictures, folderOfVideos).Result;
            PostProcessSuccessfulJobs(results.ProcessedImageJobs);
            PostProcessUnsuccessfulJobs(results.UnprocessedImageJobs);
        }

        private static void PostProcessSuccessfulJobs(IEnumerable<ImageJob> imageJobs)
        {
            PostProcessCore(imageJobs, "Successful_Snapshots");
        }
        
        private static void PostProcessUnsuccessfulJobs(IEnumerable<ImageJob> imageJobs)
        {
            PostProcessCore(imageJobs, "Unsuccessful_Snapshots");
        }
        
        private static void PostProcessCore(IEnumerable<ImageJob> imageJobs, string folderName)
        {
            if (imageJobs.Any() == false)
            {
                return;
            }

            var containingFolder = Path.GetDirectoryName(imageJobs.First().OriginalFilePath);
            var unfinishedSnapshotFolder = Path.Combine(containingFolder, folderName);
            if (Directory.Exists(unfinishedSnapshotFolder) == false)
            {
                Directory.CreateDirectory(unfinishedSnapshotFolder);
            }
            
            foreach (var imageJob in imageJobs)
            {
                var finalFileName = Path.GetFileName(imageJob.OriginalFilePath);
                TryMoveFile(imageJob.OriginalFilePath, Path.Combine(unfinishedSnapshotFolder, finalFileName));
                TryDeleteFile(imageJob.SliceImagePath);
            }
        }

        private static void TryMoveFile(string source, string dest)
        {
            try
            {
                File.Move(source, dest);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Could not move file {0} to {1}. {2}", source, dest, e.Message);
            }
        }
        
        private static void TryDeleteFile(string path)
        {
            try
            {
                File.Delete(path);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Could not delete file {0}. {1}", path, e.Message);
            }
        }
        
        private static bool ValidateFolder(string folder)
        {
            if (Directory.Exists(folder) == false)
            {
                Console.Error.WriteLine("{0} does not exist", Path.GetFullPath(folder));
                PrintHelp();
                return false;
            }

            return true;
        }

        private static void PrintHelp()
        {
            Console.Error.WriteLine("Dispatch v1.0 - Manages the processing of multiple snapshots to prevent memory and CPU overflows");
            Console.Error.WriteLine("Usage: <this executable> <folder of pictures> <folder of videos>");
        }
    }
}
