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
using System.Threading.Tasks;

namespace Indexer
{
    internal static class Indexer
    {
        #region private static fields
        private static readonly TimeSpan PLAYBACK_DURATION = TimeSpan.FromSeconds(30);
        #endregion
        
        #region public methods
        public async static Task<IEnumerable<IndexEntry>> IndexVideoAsync(string videoFile)
        {
            MediaInfo info = await GetMediaInfoAsync(videoFile);
            return await GetIndexEntriesAsync(videoFile, info, Path.GetRandomFileName());
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

        private async static Task<IEnumerable<IndexEntry>> GetIndexEntriesAsync(string videoFile, MediaInfo info, string outputDirectory)
        {
            var indexEntries = new List<IndexEntry>();
            TimeSpan totalDuration = info.GetDuration();
            for (var index = TimeSpan.FromSeconds(0); index < totalDuration; index += PLAYBACK_DURATION) {
                IEnumerable<IndexEntry> indexEntry = await GetIndexEntriesAtIndexAsync(videoFile, index, outputDirectory, info.GetFramerate(), totalDuration);
                indexEntries.AddRange(indexEntry);
            }
            
            return indexEntries;
        }
        
        private async static Task<IEnumerable<IndexEntry>> GetIndexEntriesAtIndexAsync(
            string videoFile,
            TimeSpan index,
            string outputDirectory,
            FPS framerate,
            TimeSpan totalDuration
        )
        {
            var ffmpegProcessSettings = new FFMPEGProcessSettings(
                videoFile,
                outputDirectory,
                index,
                CalculateFramesToOutputFromFramerate(index, framerate, totalDuration),
                framerate
            );
            
            if (Directory.Exists(outputDirectory) == false)
            {
                Directory.CreateDirectory(outputDirectory);
            }
            
            using(var ffmpegProcess = new FFMPEGProcess(ffmpegProcessSettings))
            {
                await ffmpegProcess.ExecuteAsync();
                var indexEntries = new List<IndexEntry>();
                foreach (string picturefile in Directory.EnumerateFiles(outputDirectory, "*.png", SearchOption.AllDirectories)) 
                {
                    ImageFingerPrinter
                        .TryCalculateFingerPrint(picturefile)
                        .Apply(fingerPrint => indexEntries.Add(
                            new IndexEntry
                            {
                                VideoFile = videoFile,
                                FrameTimeStamp = index,
                                FrameHash = fingerPrint,
                            }
                        ));
                }
                return indexEntries;
            }
        }
        
        private static int CalculateFramesToOutputFromFramerate(TimeSpan index, FPS framerate, TimeSpan totalDuration)
        {
            int numeratorMultiplier = index + PLAYBACK_DURATION < totalDuration
                ? PLAYBACK_DURATION.Seconds
                : (totalDuration - index).Seconds;

            return (framerate.Numerator * numeratorMultiplier) / framerate.Denominator;
        }
        #endregion
    }
}