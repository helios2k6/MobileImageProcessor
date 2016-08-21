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
using CommandLine;
using Functional.Maybe;
using System;
using System.Collections.Generic;
using System.Linq;
using CommonImageModel.Y4M;
using System.Drawing;

namespace Indexer
{
    [Verb("index", HelpText = "Index a video's frames")]
    internal sealed class IndexCommandVerb
    {
        [Option('i', "index-file", Required = true, HelpText = "The path to save the index file")]
        public string IndexFile { get; set; }

        [Option('v', "video-file", Required = true, HelpText = "The video to index")]
        public string VideoFile { get; set; }
    }

    [Verb("search", HelpText = "Search for a specific video and time index of when a frame occurs")]
    internal sealed class SearchCommandVerb
    {
        [Option('i', "index-file", Required = true, HelpText = "The path to save the index file")]
        public string IndexFile { get; set; }

        [Option('p', "picture-file", Required = true, HelpText = "The path to the picture you want to search for")]
        public string PictureFile { get; set; }
    }

    internal static class Driver
    {
        public static void Main(string[] args)
        {
            if (true)
            {
                var videoFileParser = new VideoFileParser(@"D:\BENCH\raw_output.y4m");
                Maybe<VideoFile> videoFile = videoFileParser.TryParseVideoFile();
                if (videoFile.IsSomething())
                {
                    VideoFile m = videoFile.Value;
                    var bitmap = new Bitmap(m.Header.Width, m.Header.Height);
                    VideoFrame firstFrame = m.Frames.First();
                    for (int row = 0; row < m.Header.Height; row++)
                    {
                        for (int col = 0; col < m.Header.Width; col++)
                        {
                            bitmap.SetPixel(col, row, firstFrame.Frame[row][col]);
                        }
                    }

                    bitmap.Save(@"D:\BENCH\raw_output_420_to_444.png");
                    bitmap.Dispose();
                }
            }
            Parser.Default.ParseArguments<SearchCommandVerb, IndexCommandVerb>(args)
                .WithParsed<SearchCommandVerb>(search =>
                {
                    var database = new IndexDatabase(search.IndexFile);
                    Maybe<IEnumerable<IndexEntry>> queryResultMaybe =
                        from fingerPrint in ImageFingerPrinter.TryCalculateFingerPrint(search.PictureFile)
                        from queryResult in database.TryFindEntries(fingerPrint)
                        select queryResult;

                    if (queryResultMaybe.HasValue)
                    {
                        IEnumerable<IndexEntry> queryResults = queryResultMaybe.Value.ToList();
                        Console.Error.WriteLine(string.Format("{0} Result(s) Found:", queryResults.Count()));
                        foreach (IndexEntry entry in queryResults)
                        {
                            Console.Error.WriteLine(string.Format("{0} @ {1}", entry.VideoFile, entry.StartTime));
                        }
                    }
                    else
                    {
                        Console.Error.WriteLine("No results found.");
                    }
                })
                .WithParsed<IndexCommandVerb>(index =>
                {
                    var database = new IndexDatabase(index.IndexFile);
                    IEnumerable<IndexEntry> indexEntries = Indexer.IndexVideoAsync(index.VideoFile).Result;
                    database.QueueAddEntries(indexEntries);
                    database.Flush();
                })
                .WithNotParsed(errors =>
                {
                });
        }
    }
}
