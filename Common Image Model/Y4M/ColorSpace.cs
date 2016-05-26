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

namespace CommonImageModel.Y4M
{
    /// <summary>
    /// Represents the different colorspaces that the video is encoded in
    /// </summary>
    public sealed class ColorSpace : IEquatable<ColorSpace>
    {
        #region private fields
        private static readonly IEnumerable<ColorSpace> ColorSpaces = new HashSet<ColorSpace>
        {
            FourTwoZeroJpeg,
            FourTwoZeroPaldv,
            FourTwoZeroMpeg2,
            FourTwoZero,
            FourTwoTwo,
            FourFourFour,
            FourTwoZeroPTen,
            FourTwoZeroPTwelve,
            FourTwoTwoPTen,
            FourTwoTwoPTwelve,
            FourFourFourPTen,
            FourFourFourPTwelve,
        };
        #endregion
        #region public fields
        /// <summary>
        /// 4:2:0 with biaxially-displaced chroma planes
        /// </summary>
        public static readonly ColorSpace FourTwoZeroJpeg = new ColorSpace("4:2:0 with biaxially-displaced chroma planes", 0, "420jpeg", 8);
        /// <summary>
        /// 4:2:0 with vertically-displaced chroma planes
        /// </summary>
        public static readonly ColorSpace FourTwoZeroPaldv = new ColorSpace("4:2:0 with vertically-displaced chroma places", 1, "420paldv", 8);
        /// <summary>
        /// 4:2:0 with vertically-displaced chroma planes (just like PAL-DV)
        /// </summary>
        public static readonly ColorSpace FourTwoZeroMpeg2 = new ColorSpace("4:2:0 with vertically-displaced chroma places", 1, "420mpeg2", 8);
        /// <summary>
        /// 4:2:0 with coincident chroma planes. This is basically the same as 420jpeg
        /// </summary>
        public static readonly ColorSpace FourTwoZero = new ColorSpace("4:2:0 with coincident chroma planes", 2, "420", 8);
        /// <summary>
        /// 4:2:2 colorspace 
        /// </summary>
        public static readonly ColorSpace FourTwoTwo = new ColorSpace("4:2:2", 3, "422", 8);
        /// <summary>
        /// 4:4:4 colorspace
        /// </summary>
        public static readonly ColorSpace FourFourFour = new ColorSpace("4:4:4", 4, "444", 8);
        /// <summary>
        /// 4:2:0 colorspace with 10-bit pixels
        /// </summary>
        public static readonly ColorSpace FourTwoZeroPTen = new ColorSpace("4:2:0 with 10-bit pixels", 5, "420p10", 10);
        /// <summary>
        /// 4:2:0 colorspace with 12-bit pixels
        /// </summary>
        public static readonly ColorSpace FourTwoZeroPTwelve = new ColorSpace("4:2:0 with 12-bit pixels", 5, "420p12", 12);
        /// <summary>
        /// 4:2:2 colorspace with 10-bit pixels
        /// </summary>
        public static readonly ColorSpace FourTwoTwoPTen = new ColorSpace("4:2:0 with 10-bit pixels", 5, "422p10", 10);
        /// <summary>
        /// 4:2:2 colorspace with 12-bit pixels
        /// </summary>
        public static readonly ColorSpace FourTwoTwoPTwelve = new ColorSpace("4:2:0 with 12-bit pixels", 5, "422p12", 12);
        /// <summary>
        /// 4:4:4 colorspace with 10-bit pixels
        /// </summary>
        public static readonly ColorSpace FourFourFourPTen = new ColorSpace("4:2:0 with 10-bit pixels", 5, "444p10", 10);
        /// <summary>
        /// 4:4:4 colorspace with 12-bit pixels
        /// </summary>
        public static readonly ColorSpace FourFourFourPTwelve = new ColorSpace("4:2:0 with 12-bit pixels", 5, "444p12", 12);
        #endregion

        #region public properties
        /// <summary>
        /// The parameter argument used when parsing
        /// </summary>
        public string ParameterArgument { get; }

        /// <summary>
        /// The name of interlacing method
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The canonical value given to this interlacing method
        /// </summary>
        public int Value { get; }

        public int BitDepth { get; }
        #endregion

        #region ctor
        private ColorSpace(string name, int value, string parameterArgument, int bitDepth)
        {
            Name = name;
            Value = value;
            ParameterArgument = parameterArgument;
            BitDepth = bitDepth;
        }
        #endregion

        #region public methods
        public override string ToString()
        {
            return Name;
        }

        public bool Equals(ColorSpace other)
        {
            return ReferenceEquals(this, other);
        }

        public override bool Equals(object other)
        {
            return Equals(other as ColorSpace);
        }

        public override int GetHashCode()
        {
            return Value;
        }

        /// <summary>
        /// Attempt to parse the colorspace parameter from the raw string
        /// </summary>
        public static Maybe<ColorSpace> TryParse(string parameter)
        {
            foreach (ColorSpace candidateColorSpace in ColorSpaces)
            {
                if (string.Equals(candidateColorSpace.ParameterArgument, parameter, StringComparison.OrdinalIgnoreCase))
                {
                    return candidateColorSpace.ToMaybe();
                }
            }

            return Maybe<ColorSpace>.Nothing;
        }
        #endregion
    }
}