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
using CommonImageModel.Y4M;
using Indexer.Media;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Indexer
{
    internal static class Indexer
    {
        #region private static fields
        private static readonly TimeSpan PlaybackDuration = TimeSpan.FromSeconds(15);
        #endregion

        #region public methods
        public static void IndexVideo(string videoFile, IndexDatabase database)
        {
            MediaInfo info = new MediaInfoProcess(videoFile).Execute();
            IndexEntries(videoFile, info, database);
        }
        #endregion

        #region private methods
        private static void IndexEntries(string videoFile, MediaInfo info, IndexDatabase database)
        {
            TimeSpan totalDuration = info.GetDuration();
            for (var startTime = TimeSpan.FromSeconds(0); startTime < totalDuration; startTime += PlaybackDuration)
            {
                IndexEntriesAtIndex(videoFile, startTime, info.GetFramerate(), totalDuration, database);
            }
        }

        private static void IndexEntriesAtIndex(
            string videoFile,
            TimeSpan startTime,
            Ratio framerate,
            TimeSpan totalDuration,
            IndexDatabase database
        )
        {
            string outputDirectory = Path.GetRandomFileName();
            Ratio halfFramerate = new Ratio(framerate.Numerator, framerate.Denominator * 2);
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
                IndexFilesInDirectory(videoFile, outputDirectory, startTime, database);
                try
                {
                    Directory.Delete(outputDirectory, true);
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(string.Format("Could not clean up images: {0}", e.Message));
                }
            }
        }

        private static void IndexFilesInDirectory(string originalFileName, string directory, TimeSpan startTime, IndexDatabase database)
        {
            foreach (string file in Directory.EnumerateFiles(directory, "*.y4m"))
            {
                new VideoFileParser(file).TryParseVideoFile().Apply(videoFile =>
                {
                    Parallel.ForEach(videoFile.Frames, frame =>
                    {
                        database.QueueAddEntry(new IndexEntry
                        {
                            VideoFile = originalFileName,
                            StartTime = startTime,
                            EndTime = startTime + PlaybackDuration,
                            FrameHash = ImageFingerPrinter.CalculateFingerPrint(frame),
                        });
                    });
                });
            }
        }

        private static int CalculateFramesToOutputFromFramerate(TimeSpan index, Ratio framerate, TimeSpan totalDuration)
        {
            int numeratorMultiplier = index + PlaybackDuration < totalDuration
                ? PlaybackDuration.Seconds
                : (totalDuration - index).Seconds;

            return (framerate.Numerator * numeratorMultiplier) / framerate.Denominator;
        }
        #endregion
    }
}