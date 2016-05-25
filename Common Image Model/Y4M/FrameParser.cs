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

using Functional.Maybe;
using System;
using System.IO;

namespace CommonImageModel.Y4M
{
    /// <summary>
    /// Parses the frame from a bitstream
    /// </summary>
    public sealed class FrameParser
    {
        #region private fields
        private static readonly byte EndFrameByteMark = 0x0A;
        private readonly Header _header;
        #endregion

        #region private properties
        private ColorSpace DetectedColorSpace
        {
            get
            {
                return _header.ColorSpace.HasValue
                    ? _header.ColorSpace.Value
                    : ColorSpace.FourTwoZero;
            }
        }
        #endregion

        #region ctor
        public FrameParser(Header header)
        {
            _header = header;
        }
        #endregion

        #region public methods
        public Maybe<VideoFrame> TryParseVideoFrame(Stream rawStream)
        {
            using (var rewindGuard = new RewindGuard(rawStream))
            {

            }

            return Maybe<VideoFrame>.Nothing;
        }
        #endregion

        #region private methods
        private Maybe<byte[]> ReadLumaPlane(Stream rawStream)
        {
            var lumaPlaneBuffer = new byte[_header.Width * _header.Height];
            int readBytes = rawStream.Read(lumaPlaneBuffer, 0, _header.Width * _header.Height);
            if (readBytes != _header.Width * _header.Height)
            {
                return Maybe<byte[]>.Nothing;
            }

            return lumaPlaneBuffer.ToMaybe();
        }

        private Maybe<byte[]> ReadChromaPlane(Stream rawStream)
        {
            if (Equals(DetectedColorSpace, ColorSpace.FourFourFour))
            {
                // 4:4:4
                return ReadPlane(rawStream, _header.Width * _header.Height);
            }
            else if (Equals(DetectedColorSpace, ColorSpace.FourTwoTwo))
            {
                // 4:2:2
                return ReadPlane(rawStream, (_header.Width * _header.Height) / 2);
            }
            else
            {
                // 4:2:0
                return ReadPlane(rawStream, (_header.Width * _header.Height) / 6);
            }
        }

        private static Maybe<byte[]> ReadPlane(Stream rawStream, int length)
        {
            var planeBuffer = new byte[length];
            int readBytes = rawStream.Read(planeBuffer, 0, length);
            if (readBytes != length)
            {
                return Maybe<byte[]>.Nothing;
            }

            return planeBuffer.ToMaybe();
        }
        #endregion
    }
}
