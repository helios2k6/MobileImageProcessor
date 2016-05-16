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
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CommonImageModel.Y4M
{
    /// <summary>
    /// Represents the Y4M file header and includes all of the information that is 
    /// associated with the Y4M file
    /// </summary>
    public sealed class Header : IEquatable<Header>
    {
        #region private fields
        private const string FileHeaderMagicTag = "YUV4MPEG2";
        private const string FrameHeaderMagicTag = "FRAME";
        private const string ParameterSeparator = " ";
        private const byte FrameHeaderEndByte = 0x0A;
        private const char CommentParameter = 'X';
        private const char WidthParameter = 'W';
        private const char HeightParemter = 'H';
        private const char FrameRateParameter = 'F';
        private const char InterlaceParameter = 'I';
        private const char PixelAspectRatioParameter = 'A';
        private const char ColorSpaceParameter = 'C';
        private static readonly ICollection<char> ParameterSet = new HashSet<char>
        {
            CommentParameter,
            WidthParameter,
            HeightParemter,
            FrameRateParameter,
            InterlaceParameter,
            PixelAspectRatioParameter,
            ColorSpaceParameter,
        };
        #endregion

        #region private types
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
        #endregion

        #region public properties
        /// <summary>
        /// The type of header this represents
        /// </summary>
        public Type HeaderType { get; private set; }

        /// <summary>
        /// The width of the video
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// The height of the video
        /// </summary> 
        public int Height { get; private set; }

        /// <summary>
        /// The framerate of the video
        /// </summary>
        public Ratio Framerate { get; private set; }

        /// <summary>
        /// The pixel aspect ratio of the video, if it's available
        /// </summary>
        public Maybe<Ratio> PixelAspectRatio { get; private set; }

        /// <summary>
        /// Represents the colorspace this video uses
        /// </summary>
        public Maybe<ColorSpace> ColorSpace { get; private set; }

        /// <summary>
        /// The interlacing setting used
        /// </summary>
        public Maybe<Interlacing> Interlacing { get; private set; }

        /// <summary>
        /// The comments that accompanied this header
        /// </summary>
        public IEnumerable<string> Comments { get; private set; }
        #endregion

        #region ctor
        /// <summary>
        /// Construct a default header
        /// </summary>
        /// <remarks>
        /// This is intentionally private to ensure that the header is constructed properly
        /// </remarks>
        private Header()
        {
            HeaderType = Type.YUV4MPEG;
            Width = -1;
            Height = -1;
            PixelAspectRatio = Maybe<Ratio>.Nothing;
            ColorSpace = Maybe<ColorSpace>.Nothing;
            Framerate = new Ratio();
            Comments = Enumerable.Empty<string>();
        }
        #endregion

        #region public methods
        public bool Equals(Header other)
        {
            if (EqualsPreamble(other) == false)
            {
                return false;
            }

            return Equals(HeaderType, other.HeaderType) &&
                Equals(Width, other.Width) &&
                Equals(Height, other.Height) &&
                Equals(Framerate, other.Framerate) &&
                Equals(PixelAspectRatio, other.PixelAspectRatio) &&
                Equals(ColorSpace, other.ColorSpace) &&
                Enumerable.SequenceEqual(Comments, other.Comments);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Header);
        }

        public override int GetHashCode()
        {
            return HeaderType.GetHashCode() ^
                Width.GetHashCode() ^
                Height.GetHashCode() ^
                Framerate.GetHashCode() ^
                PixelAspectRatio.GetHashCode() ^
                ColorSpace.GetHashCode() ^
                Comments.Aggregate(0, (agg, s) => agg ^ s.GetHashCode(), i => i);
        }

        public static Maybe<Header> TryParseFileHeader(Stream rawStream)
        {
            if (TryReadHeaderMagicTag(rawStream, FileHeaderMagicTag) == false)
            {
                return Maybe<Header>.Nothing;
            }

            return Maybe<Header>.Nothing;
        }

        public static Maybe<Header> TryParseFrameHeader(Stream rawStream)
        {
            if (TryReadHeaderMagicTag(rawStream, FrameHeaderMagicTag) == false)
            {
                return Maybe<Header>.Nothing;
            }

            return Maybe<Header>.Nothing;
        }
        #endregion

        #region private methods
        private bool EqualsPreamble(object other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            if (GetType() != other.GetType()) return false;

            return true;
        }

        private static bool TryReadHeaderMagicTag(Stream rawStream, string headerMagicTag)
        {
            var buffer = new byte[10];
            int readBytes = rawStream.Read(buffer, 0, headerMagicTag.Length);

            if (readBytes == headerMagicTag.Length)
            {
                var readHeader = new string(buffer.Select(Convert.ToChar).ToArray());
                if (string.Equals(headerMagicTag, readHeader, StringComparison.Ordinal))
                {
                    return true;
                }
            }
            // Reset header and return false if the header doesn't match
            rawStream.Position = rawStream.Position - readBytes;
            return false;
        }

        /// <summary>
        /// Attempts to parse the parameters of the stream to form a header. If the stream is malformed, this
        /// returns None
        /// </summary>
        /// <param name="rawStream">The raw stream of bytes to parse</param>
        /// <returns>An optional Header</returns>
        private static Maybe<Header> TryReadHeaderParameters(Stream rawStream)
        {
            Maybe<IEnumerable<string>> maybeParameters = TryGetParameters(rawStream);
            if (maybeParameters.IsNothing())
            {
                return Maybe<Header>.Nothing;
            }

            IEnumerable<string> comments = Enumerable.Empty<string>();
            Maybe<int> width = Maybe<int>.Nothing;
            Maybe<int> height = Maybe<int>.Nothing;
            Maybe<Ratio> framerate = Maybe<Ratio>.Nothing;
            Maybe<Interlacing> interlacing = Maybe<Interlacing>.Nothing;

            foreach (string currentFullParameter in maybeParameters.Value)
            {
                char parameter = currentFullParameter.First();
                string parameterBody = currentFullParameter.Substring(1);
                switch (parameter)
                {
                    case CommentParameter:
                        {
                            comments = comments.Append(parameterBody);
                        }
                        break;
                    case WidthParameter:
                        {
                            int possibleWidth = -1;
                            if (int.TryParse(parameterBody, out possibleWidth))
                            {
                                width = possibleWidth.ToMaybe();
                            }
                        }
                        break;
                    case HeightParemter:
                        {
                            int possibleHeight = -1;
                            if (int.TryParse(parameterBody, out possibleHeight))
                            {
                                height = possibleHeight.ToMaybe();
                            }
                        }
                        break;
                    case FrameRateParameter:
                        {
                            string[] splitFramerate = parameterBody.Split(new[] { ':' });
                            if (splitFramerate.Length == 2)
                            {
                                int numerator = -1, denominator = -1;
                                if (int.TryParse(splitFramerate[0], out numerator) && int.TryParse(splitFramerate[1], out denominator))
                                {
                                    framerate = new Ratio(numerator, denominator).ToMaybe();
                                }
                            }
                        }
                        break;
                    case InterlaceParameter:
                        {
                            interlacing = Y4M.Interlacing.TryParseInterlacing(parameterBody);
                        }
                        break;
                    case PixelAspectRatioParameter:
                        break;
                    case ColorSpaceParameter:
                        break;
                    default:
                        // Do nothing for invalid parameters
                        break;
                }
            }
            return Maybe<Header>.Nothing;
        }

        private static Maybe<IEnumerable<string>> TryGetParameters(Stream rawStream)
        {
            return Maybe<IEnumerable<string>>.Nothing;
        }

        private static Maybe<string> TryGetNextParameter(Stream rawStream)
        {
            return Maybe<string>.Nothing;
        }
        #endregion
    }
}
