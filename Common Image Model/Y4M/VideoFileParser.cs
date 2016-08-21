﻿/* 
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

using Functional.Maybe;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CommonImageModel.Y4M
{
    /// <summary>
    /// Parses a video file from a bitstream
    /// </summary>
    public sealed class VideoFileParser
    {
        #region private fields
        private readonly string _videoFilePath;
        #endregion

        #region ctor
        /// <summary>
        /// Construct a video file parser from the given file path
        /// </summary>
        /// <param name="videoFilePath">The path to the video file</param>
        public VideoFileParser(string videoFilePath)
        {
            _videoFilePath = videoFilePath;
        }
        #endregion

        #region public methods
        /// <summary>
        /// Attempts to parse a video file
        /// </summary>
        /// <returns>A newly parsed video file or None</returns>
        public Maybe<VideoFile> TryParseVideoFile()
        {
            using (var stream = new FileStream(_videoFilePath, FileMode.Open, FileAccess.Read))
            {
                // First read file header
                Maybe<Header> fileHeader = FileHeaderParser.Instance.TryParseHeader(stream);
                if (fileHeader.IsNothing())
                {
                    return Maybe<VideoFile>.Nothing;
                }

                // Second read the frames
                Maybe<VideoFrame> parsedVideoFrame = Maybe<VideoFrame>.Nothing;
                var videoFrames = new List<VideoFrame>();
                do
                {
                    parsedVideoFrame = new VideoFrameParser(fileHeader.Value).TryParseVideoFrame(stream);
                    parsedVideoFrame.Apply(videoFrame =>
                    {
                        videoFrames.Add(parsedVideoFrame.Value);
                    });
                } while (parsedVideoFrame.IsSomething());

                if (videoFrames.Any())
                {
                    return new VideoFile(fileHeader.Value, videoFrames).ToMaybe();
                }
            }

            return Maybe<VideoFile>.Nothing;
        }
        #endregion
    }
}
