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

using CommandLine;
using CommandLine.Text;
using CommonImageModel;
using Functional.Maybe;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Slice
{
    internal sealed class CommandLineOptions
    {
        [ValueList(typeof(List<string>))]
        public List<string> InputFiles { get; set; }

        [HelpOption(HelpText = "Display this help text")]
        public string GetUsage()
        {
            var help = new HelpText
            {
                Heading = new HeadingInfo("Slice", "1.0"),
                Copyright = new CopyrightInfo("Andrew Johnson", 2015),
                AdditionalNewLineAfterOption = true,
                AddDashesToOption = true,
            };

            help.AddPreOptionsLine("Usage: <this app> (list of image files)");
            help.AddOptions(this);
            return help.ToString();
        }
    }

    /// <summary>
    /// The main entrypoint for the Slice program
    /// </summary>
    public static class Driver
    {
        public static void Main(string[] args)
        {
            var parser = new Parser();
            var options = new CommandLineOptions();
            var parseSuccess = parser.ParseArguments(args, options);
            if (parseSuccess)
            {
                IEnumerable<ImageSliceContext> results = ImageProcessor.ProcessFiles(options.InputFiles);
                IEnumerable<Maybe<ImageJob>> imageJobsMaybe = results.Select(Convert);
                var imageJobs = new ImageJobs
                {
                    Images = imageJobsMaybe.SelectWhereValueExist(t => t).ToArray(),
                };
                string serializedString = JsonConvert.SerializeObject(imageJobs, Formatting.None);
                Console.Write(serializedString);
            }
            else
            {
                Console.Error.WriteLine(options.GetUsage());
            }
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
