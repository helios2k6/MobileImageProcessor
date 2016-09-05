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
using CommonImageModel;
using Functional.Maybe;
using System;
using System.Collections.Generic;
using static Indexer.IndexDatabase;

namespace Indexer
{
    [Verb("index", HelpText = "Index a video's frames")]
    internal sealed class IndexCommandVerb
    {
        [Option('i', "index-file", Required = true, HelpText = "The path to save the index file")]
        public string IndexFile { get; set; }

        [Option('v', "video-file", Required = true, HelpText = "The video to index")]
        public string VideoFile { get; set; }

        [Option('s', "serialization-method", Required = false, HelpText = "The serialization method for the index file")]
        public string SerializationMethod { get; set; }
    }

    [Verb("search", HelpText = "Search for a specific video and time index of when a frame occurs")]
    internal sealed class SearchCommandVerb
    {
        [Option('i', "index-file", Required = true, HelpText = "The path to save the index file")]
        public string IndexFile { get; set; }

        [Option('p', "picture-file", Required = true, HelpText = "The path to the picture you want to search for")]
        public string PictureFile { get; set; }

        [Option('s', "serialization-method", Required = false, HelpText = "The serialization method for the index file")]
        public string SerializationMethod { get; set; }
    }

    internal static class Driver
    {
        public static void Main(string[] args)
        {

            Parser.Default.ParseArguments<SearchCommandVerb, IndexCommandVerb>(args)
                .WithParsed<SearchCommandVerb>(search =>
                {
                    var serializationMethod = SerializationMethod.JSON;
                    Enum.TryParse(search.SerializationMethod, out serializationMethod);
                    Maybe<ImageFingerPrint> fingerPrintMaybe = ImageFingerPrinter.TryCalculateFingerPrint(search.PictureFile);
                    if (fingerPrintMaybe.IsSomething())
                    {
                        var database = new IndexDatabase(search.IndexFile, serializationMethod);
                        IEnumerable<IndexEntry> queryResults = database.TryFindEntries(fingerPrintMaybe.Value);
                        foreach (IndexEntry entry in queryResults)
                        {
                            Console.Error.WriteLine(string.Format("{0} ({1}, {2}) ", entry.VideoFile, entry.StartTime, entry.EndTime));
                        }
                    }
                })
                .WithParsed<IndexCommandVerb>(index =>
                {
                    var serializationMethod = SerializationMethod.JSON;
                    Enum.TryParse(index.SerializationMethod, out serializationMethod);
                    var database = new IndexDatabase(index.IndexFile, serializationMethod);
                    Indexer.IndexVideo(index.VideoFile, database);
                    database.Flush();
                })
                .WithNotParsed(errors =>
                {
                    Console.Error.WriteLine("Could not parse arguments");
                });
        }
    }
}
