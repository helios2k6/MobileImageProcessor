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

using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;
using System.Text;

namespace CommonImageModel
{
    /// <summary>
    /// Represents an image fingerprint that can be used to compare it against other fingerprints
    /// </summary>
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class ImageFingerPrint : IEquatable<ImageFingerPrint>, ISerializable
    {
        #region public properties
        [JsonProperty(PropertyName = "TopLeft", Required = Required.Always)]
        public Macroblock TopLeft { get; set; }

        [JsonProperty(PropertyName = "TopRight", Required = Required.Always)]
        public Macroblock TopRight { get; set; }

        [JsonProperty(PropertyName = "Center", Required = Required.Always)]
        public Macroblock Center { get; set; }

        [JsonProperty(PropertyName = "BottomLeft", Required = Required.Always)]
        public Macroblock BottomLeft { get; set; }

        [JsonProperty(PropertyName = "BottomRight", Required = Required.Always)]
        public Macroblock BottomRight { get; set; }

        [JsonProperty(PropertyName = "FocusSquareTopLeft", Required = Required.Always)]
        public Macroblock FocusSquareTopLeft { get; set; }

        [JsonProperty(PropertyName = "FocusSquareTopRight", Required = Required.Always)]
        public Macroblock FocusSquareTopRight { get; set; }

        [JsonProperty(PropertyName = "FocusSquareBottomLeft", Required = Required.Always)]
        public Macroblock FocusSquareBottomLeft { get; set; }

        [JsonProperty(PropertyName = "FocusSquareBottomRight", Required = Required.Always)]
        public Macroblock FocusSquareBottomRight { get; set; }
        #endregion

        #region ctor
        /// <summary>
        /// Create a new Fingerprint based on the provided Macroblocks
        /// </summary>
        public ImageFingerPrint(
            Macroblock topLeft,
            Macroblock topRight,
            Macroblock center,
            Macroblock bottomLeft,
            Macroblock bottomRight,
            Macroblock focusSquareTopLeft,
            Macroblock focusSquareTopRight,
            Macroblock focusSquareBottomLeft,
            Macroblock focusSquareBottomRight
        )
        {
            TopLeft = topLeft;
            TopRight = topRight;
            Center = center;
            BottomLeft = bottomLeft;
            BottomRight = bottomRight;

            FocusSquareTopLeft = focusSquareBottomLeft;
            FocusSquareTopRight = focusSquareTopRight;
            FocusSquareBottomLeft = focusSquareBottomLeft;
            FocusSquareBottomRight = focusSquareBottomRight;
        }

        /// <summary>
        /// Default constructor for a Fingerprint object
        /// </summary>
        public ImageFingerPrint()
        {
        }

        public ImageFingerPrint(SerializationInfo info, StreamingContext context)
        {
            TopLeft = (Macroblock)info.GetValue("TopLeft", typeof(Macroblock));
            TopRight = (Macroblock)info.GetValue("TopRight", typeof(Macroblock));
            Center = (Macroblock)info.GetValue("Center", typeof(Macroblock));
            BottomLeft = (Macroblock)info.GetValue("BottomLeft", typeof(Macroblock));
            BottomRight = (Macroblock)info.GetValue("BottomRight", typeof(Macroblock));
            FocusSquareTopLeft = (Macroblock)info.GetValue("FocusSquareTopLeft", typeof(Macroblock));
            FocusSquareTopRight = (Macroblock)info.GetValue("FocusSquareTopRight", typeof(Macroblock));
            FocusSquareBottomLeft = (Macroblock)info.GetValue("FocusSquareBottomLeft", typeof(Macroblock));
            FocusSquareBottomRight = (Macroblock)info.GetValue("FocusSquareBottomRight", typeof(Macroblock));
        }
        #endregion

        #region public methods
        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder
                .Append("Image FingerPrint with: ")
                .AppendFormat("Top Left {0} - ", TopLeft)
                .AppendFormat("Top Right {0} - ", TopRight)
                .AppendFormat("Center {0} - ", Center)
                .AppendFormat("Bottom Left {0} - ", BottomLeft)
                .AppendFormat("Bottom Right {0} - ", BottomRight)
                .AppendFormat("Focus Square Top Left {0} - ", FocusSquareTopLeft)
                .AppendFormat("Focus Square Top Right {0} - ", FocusSquareTopRight)
                .AppendFormat("Focus Square Bottom Left {0} - ", FocusSquareBottomLeft)
                .AppendFormat("Focus Square Bottom Right {0}", FocusSquareBottomRight);

            return stringBuilder.ToString();
        }

        public bool Equals(ImageFingerPrint other)
        {
            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Equals(TopLeft, other.TopLeft) &&
                Equals(TopRight, other.TopRight) &&
                Equals(Center, other.Center) &&
                Equals(BottomLeft, other.BottomLeft) &&
                Equals(BottomRight, other.BottomRight) &&
                Equals(FocusSquareTopLeft, other.FocusSquareTopLeft) &&
                Equals(FocusSquareTopRight, other.FocusSquareTopRight) &&
                Equals(FocusSquareBottomLeft, other.FocusSquareBottomLeft) &&
                Equals(FocusSquareBottomRight, other.FocusSquareBottomRight);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return Equals(obj as ImageFingerPrint);
        }

        public override int GetHashCode()
        {
            return TopLeft.GetHashCode() ^
                TopRight.GetHashCode() ^
                Center.GetHashCode() ^
                BottomLeft.GetHashCode() ^
                BottomRight.GetHashCode() ^
                FocusSquareTopLeft.GetHashCode() ^
                FocusSquareTopRight.GetHashCode() ^
                FocusSquareBottomLeft.GetHashCode() ^
                FocusSquareBottomRight.GetHashCode();
        }

        public bool IsSimilarTo(ImageFingerPrint other)
        {
            return TopLeft.IsSimilarTo(other.TopLeft) &&
                TopRight.IsSimilarTo(other.TopRight) &&
                Center.IsSimilarTo(other.Center) &&
                BottomLeft.IsSimilarTo(other.BottomLeft) &&
                BottomRight.IsSimilarTo(other.BottomRight) &&
                FocusSquareTopLeft.IsSimilarTo(other.FocusSquareTopLeft) &&
                FocusSquareTopRight.IsSimilarTo(other.FocusSquareTopRight) &&
                FocusSquareBottomLeft.IsSimilarTo(other.FocusSquareBottomLeft) &&
                FocusSquareBottomRight.IsSimilarTo(other.FocusSquareBottomRight);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("TopLeft", TopLeft);
            info.AddValue("TopRight", TopRight);
            info.AddValue("Center", Center);
            info.AddValue("BottomLeft", BottomLeft);
            info.AddValue("BottomRight", BottomRight);
            info.AddValue("FocusSquareTopLeft", FocusSquareTopLeft);
            info.AddValue("FocusSquareTopRight", FocusSquareTopRight);
            info.AddValue("FocusSquareBottomLeft", FocusSquareBottomLeft);
            info.AddValue("FocusSquareBottomRight", FocusSquareBottomRight);
        }
        #endregion
    }
}
