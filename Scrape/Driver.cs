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
using System.Linq;
using System.Text;

namespace Scrape
{
    /// <summary>
    /// Entry point for the Scrape process
    /// </summary>
    internal static class Driver
    {
        /// <summary>
        /// Main method that executes on startup 
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            var imageJobs = CommonFunctions.TryReadStandardIn();
            if (imageJobs.IsNothing())
            {
                PrintHelp();
                return;
            }
            var processedImageJobs = ScrapeJobProcessor.ProcessImageJobs(imageJobs.Value);
            var newImageModel = new ImageJobs
            {
                Images = processedImageJobs.ToArray(),
            };

            Console.WriteLine(JsonConvert.SerializeObject(newImageModel));
            CommonFunctions.CloseAllStandardFileHandles();
        }

        private static void PrintHelp()
        {
            var builder = new StringBuilder();
            builder.AppendLine("Scrape 1.0")
                .AppendLine("Usage: <this program> <JSON Input>");
            Console.Error.WriteLine(builder.ToString());
        }
    }
}
