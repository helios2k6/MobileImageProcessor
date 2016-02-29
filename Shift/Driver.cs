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

namespace Shift
{
    internal static class Driver
    {
        public static void Main(string[] args)
        {
            Maybe<int> timeShiftAmount = TryGetTimeShift(args);
            if (timeShiftAmount.IsNothing())
            {
                PrintHelp();
                return;
            }

            Maybe<ImageJobs> imageJobs = CommonFunctions.TryReadStandardIn();
            if (imageJobs.IsNothing())
            {
                PrintHelp();
                return;
            }

            ImageJobs shiftedImageJobs = ShiftAllJobs(imageJobs.Value, timeShiftAmount.Value);
            Console.WriteLine(JsonConvert.SerializeObject(shiftedImageJobs));
            CommonFunctions.CloseAllStandardFileHandles();
        }

        private static void PrintHelp()
        {
            Console.Error.WriteLine("Usage: <this app> <number of seconds to timeshift>");
        }

        private static Maybe<int> TryGetTimeShift(string[] args)
        {
            if (args.Length > 0)
            {
                string arg = args[0];
                int parsedInt;
                if (int.TryParse(arg, out parsedInt))
                {
                    return parsedInt.ToMaybe();
                }
            }

            return Maybe<int>.Nothing;
        }

        private static ImageJobs ShiftAllJobs(ImageJobs oldImageJobs, int secondsToShift)
        {
            var shiftedImageJobList = new List<ImageJob>();
            foreach (ImageJob job in oldImageJobs.Images)
            {
                shiftedImageJobList.Add(new ImageJob
                {
                    ImageSnapshots = job.ImageSnapshots,
                    OriginalFilePath = job.OriginalFilePath,
                    SliceImagePath = job.SliceImagePath,
                    SnapshotTimestamp = job.SnapshotTimestamp + new TimeSpan(0, 0, secondsToShift),
                });
            }

            return new ImageJobs
            {
                Images = shiftedImageJobList.ToArray(),
            };
        }
    }
}
