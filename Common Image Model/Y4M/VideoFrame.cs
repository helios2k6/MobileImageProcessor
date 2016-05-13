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

using System;
using System.Collections.Generic;

namespace CommonImageModel.Y4M
{
    /// <summary>
    /// Represents a single frame of video
    /// </summary>
    public sealed class VideoFrame
    {
        #region private fields
        private readonly Lazy<IEnumerable<byte>> _bytes;
        #endregion

        #region public properties
        public IEnumerable<byte> Bytes
        {
            get { return _bytes.Value; }
        }
        #endregion

        #region ctor
        public VideoFrame(IEnumerable<byte> rawBytes)
        {
            _bytes = new Lazy<IEnumerable<byte>>(() => GenerateFrame(rawBytes));
        }
        #endregion

        #region public methods
        #endregion

        #region private methods
        private static IEnumerable<byte> GenerateFrame(IEnumerable<byte> rawBytes)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
