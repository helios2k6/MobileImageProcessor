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
using System;
using System.Collections.Generic;
using System.Linq;

namespace Scrape
{
    /// <summary>
    /// Processes Image Jobs 
    /// </summary>
    internal static class ScrapeJobProcessor
    {
        private const int STANDARD_FPS_NUMERATOR = 24000;
        private const int STANDARD_FPS_DENOMINATOR = 1001;
        
        public static IEnumerable<ImageJob> ProcessImageJobs(ImageJobs imageJobs)
        {
            return imageJobs
                    .Images
                    .Select(TryProcessImageJob)
                    .SelectWhereValueExist(i => i);
        }

        private static Maybe<ImageJob> TryProcessImageJob(ImageJob oldImageJob)
        {
            return from timeSpan in TryGetTimeSpan(oldImageJob)
                   select new ImageJob
                   {
                       OriginalFilePath = oldImageJob.OriginalFilePath,
                       SliceImagePath = oldImageJob.SliceImagePath,
                       FrameNumber = CalculateFrameNumber(timeSpan),
                   };
        }

        private static int CalculateFrameNumber(TimeSpan timeSpan)
        {
            var numberOfSeconds = (timeSpan.Hours * 60 * 60) + 
                (timeSpan.Minutes * 60) +
                timeSpan.Seconds;
            
            return (int)Math.Floor(
                ((double)(numberOfSeconds * STANDARD_FPS_NUMERATOR)) / 
                STANDARD_FPS_DENOMINATOR
            );
        }

        private static Maybe<TimeSpan> TryGetTimeSpan(ImageJob imageJob)
        {
            var tesseractProcess = new TesseractProcess(imageJob.SliceImagePath);
            var tesseractOutput = tesseractProcess.Execute();
            return OutputFileProcessor.TryGetTime(tesseractOutput);
        }
    }
}