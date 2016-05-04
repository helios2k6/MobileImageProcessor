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
using Indexer.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Indexer
{
    internal static class Indexer
    {
        #region private static fields
        private static readonly TimeSpan PlaybackDuration = TimeSpan.FromSeconds(30);
        #endregion

        #region public methods
        public async static Task<IEnumerable<IndexEntry>> IndexVideoAsync(string videoFile)
        {
            MediaInfo info = await GetMediaInfoAsync(videoFile);
            return GetIndexEntries(videoFile, info);
        }
        #endregion

        #region private methods
        private static Task<MediaInfo> GetMediaInfoAsync(string videoFile)
        {
            return Task.Factory.StartNew(() =>
            {
                return (new MediaInfoProcess(videoFile)).Execute();
            });
        }

        private static IEnumerable<IndexEntry> GetIndexEntries(string videoFile, MediaInfo info)
        {
            TimeSpan totalDuration = info.GetDuration();
            return from startTime in GenerateStartTimeSpans(totalDuration).AsParallel()
                   from indexEntriesForPhoto in GetIndexEntriesAtIndex(videoFile, startTime, info.GetFramerate(), totalDuration)
                   select indexEntriesForPhoto;
        }

        private static IEnumerable<TimeSpan> GenerateStartTimeSpans(TimeSpan totalDuration)
        {
            for (var startTime = TimeSpan.FromSeconds(0); startTime < totalDuration; startTime += PlaybackDuration)
            {
                yield return startTime;
            }
        }

        private static IEnumerable<IndexEntry> GetIndexEntriesAtIndex(
            string videoFile,
            TimeSpan startTime,
            FPS framerate,
            TimeSpan totalDuration
        )
        {
            string outputDirectory = Path.GetRandomFileName();
            FPS halfFramerate = new FPS(framerate.Numerator, framerate.Denominator * 2);
            var ffmpegProcessSettings = new FFMPEGProcessSettings(
                videoFile,
                outputDirectory,
                startTime,
                CalculateFramesToOutputFromFramerate(startTime, halfFramerate, totalDuration),
                framerate,
                FFMPEGOutputFormat.Y4M
            );

            if (Directory.Exists(outputDirectory) == false)
            {
                Directory.CreateDirectory(outputDirectory);
            }

            using (var ffmpegProcess = new FFMPEGProcess(ffmpegProcessSettings))
            {
                ffmpegProcess.Execute();
                var indexEntries = new List<IndexEntry>();
                foreach (string picturefile in Directory.EnumerateFiles(outputDirectory, "*.png", SearchOption.AllDirectories))
                {
                    
                    // TODO: Decode raw Y4M format
                    /*
                    ImageFingerPrinter
                        .TryCalculateFingerPrint(picturefile)
                        .Apply(fingerPrint => indexEntries.Add(
                            new IndexEntry
                            {
                                VideoFile = videoFile,
                                StartTime = startTime,
                                EndTime = startTime + PlaybackDuration,
                                FrameHash = fingerPrint,
                            }
                        ));*/
                }

                try
                {
                    Directory.Delete(outputDirectory, true);
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(string.Format("Could not clean up images: {0}", e.Message));
                }
                return indexEntries;
            }
        }

        private static int CalculateFramesToOutputFromFramerate(TimeSpan index, FPS framerate, TimeSpan totalDuration)
        {
            int numeratorMultiplier = index + PlaybackDuration < totalDuration
                ? PlaybackDuration.Seconds
                : (totalDuration - index).Seconds;

            return (framerate.Numerator * numeratorMultiplier) / framerate.Denominator;
        }
        #endregion
    }
}