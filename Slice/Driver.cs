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

namespace Slice
{
    /// <summary>
    /// The main entrypoint for the Slice program
    /// </summary>
    public static class Driver
    {
        public static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                PrintHelp();
                return;
            }

            IEnumerable<ImageSliceContext> processedSlices = ImageProcessor.ProcessFiles(args);
            IEnumerable<Maybe<ImageJob>> imageJobsMaybe = processedSlices.Select(Convert);
            var imageJobs = new ImageJobs
            {
                Images = imageJobsMaybe.SelectWhereValueExist(t => t).ToArray(),
            };
            Console.WriteLine(JsonConvert.SerializeObject(imageJobs, Formatting.None));

            CommonFunctions.CloseAllStandardFileHandles();
        }

        private static void PrintHelp()
        {
            Console.Error.WriteLine("Slice 1.0");
            Console.Error.WriteLine("Usage: <this app> <list of pictures to analyze and slice");
        }

        private static Maybe<ImageJob> Convert(ImageSliceContext context)
        {
            return from path in context.OriginalFile
                   from sliceImagePath in context.SlicedImageFile
                   select new ImageJob
                   {
                       OriginalFilePath = path,
                       SliceImagePath = sliceImagePath,
                   };
        }
    }
}
