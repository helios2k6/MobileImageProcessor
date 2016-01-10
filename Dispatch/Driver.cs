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

using System;
using System.IO;

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

            Dispatcher.DispatchAllProcessesAsync(folderOfPictures, folderOfVideos).Wait();
        }

        private static bool ValidateFolder(string folder)
        {
            if (Directory.Exists(folder) == false)
            {
                Console.Error.WriteLine("{0} does not exist", folder);
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
