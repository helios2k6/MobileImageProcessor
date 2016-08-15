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

using System.Drawing;

namespace CommonImageModel.Y4M
{
    /// <summary>
    /// Represents a single frame of video
    /// </summary>
    public sealed class VideoFrame
    {
        #region public properties
        /// <summary>
        /// The header for this frame
        /// </summary>
        public Header Header { get; }

        /// <summary>
        /// The color pixels of the frame
        /// </summary>
        public Color[][] Frame { get; }
        #endregion

        #region ctor
        /// <summary>
        /// Construct a new VideoFrame
        /// </summary>
        /// <param name="header">The frame header</param>
        /// <param name="frame">The actual color matrix</param>
        public VideoFrame(Header header, Color[][] frame)
        {
            Header = header;
            Frame = frame;
        }
        #endregion
    }
}
