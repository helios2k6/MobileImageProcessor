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
using System.IO;

namespace CommonImageModel.Y4M
{
    /// <summary>
    /// Represents the Y4M file header and includes all of the information that is 
    /// associated with the Y4M file
    /// </summary>
    public sealed class Header
    {
        /// <summary>
        //// Represents the header type  
        /// </summary>
        public enum Type
        {
            /// <summary>
            /// The file level header
            /// </summary>
            YUV4MPEG,
            /// <summary>
            /// The frame level header
            /// </summary>
            FRAME,
        }

        #region public properties
        /// <summary>
        /// The type of header this represents
        /// </summary>
        public Header.Type HeaderType { get; }

        /// <summary>
        /// The width of the video
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// The height of the video
        /// </summary> 
        public int Height { get; }

        /// <summary>
        /// The framerate of the video
        /// </summary>
        public FPS Framerate { get; }

        /// <summary>
        /// The pixel aspect ratio of the video, if it's available
        /// </summary>
        public Maybe<PixelAspectRatio> PixelAspectRatio { get; }

        /// <summary>
        /// Represents the colorspace this video uses
        /// </summary>
        public Maybe<ColorSpace> ColorSpace { get; }
        #endregion

        #region ctor
        /// <summary>
        /// Construct a new Y4M file header form the raw bytes
        /// </summary>
        private Header(Header.Type headerType, int width, int height, FPS framerate, Maybe<PixelAspectRatio> pixelAspectRatio, Maybe<ColorSpace> colorSpace)
        {
            HeaderType = headerType;
            Width = width;
            Height = height;
            Framerate = framerate;
            PixelAspectRatio = pixelAspectRatio;
            ColorSpace = colorSpace;
        }
        #endregion

        #region public methods
        public static Maybe<Header> TryParseFileHeader(Stream rawStream)
        {
            return Maybe<Header>.Nothing;
        }
        
        public static Maybe<Header> TryParseFrameHeader(Stream rawStream)
        {
            return Maybe<Header>.Nothing;
        }
        #endregion

        #region private methods
        #endregion
    }
}
