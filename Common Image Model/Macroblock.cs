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
using System.Drawing;
using System.Runtime.Serialization;

namespace CommonImageModel
{
    /// <summary>
    /// Represents a contiguous grid of colors
    /// </summary>
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class Macroblock : IEquatable<Macroblock>, ISerializable
    {
        #region private fields
        private readonly Lazy<int> _hashCode;
        private readonly Lazy<Color> _averageColor;
        #endregion

        #region public properties
        /// <summary>
        /// The width of the Macroblock
        /// </summary>
        public int Width
        {
            get { return ColorGrid.GetLength(0); }
        }

        /// <summary>
        /// The height of the Macroblock
        /// </summary>
        public int Height
        {
            get { return ColorGrid.GetLength(1); }
        }

        /// <summary>
        /// The actual grid of colors itself
        /// </summary>
        [JsonProperty(PropertyName = "ColorGrid", Required = Required.Always)]
        public Color[,] ColorGrid { get; set; }
        #endregion

        #region ctor
        /// <summary>
        /// Constructs a new Macroblock 
        /// </summary>
        public Macroblock()
        {
            _hashCode = new Lazy<int>(CalculateHashCode);
            _averageColor = new Lazy<Color>(CalculateAverageColor);
        }

        /// <summary>
        /// Constructs a new Macroblock with the provided two dimensional color grid as 
        /// the Macroblock
        /// </summary>
        /// <param name="colorGrid"></param>
        public Macroblock(Color[,] colorGrid)
            : this()
        {
            ColorGrid = colorGrid;
        }

        public Macroblock(SerializationInfo info, StreamingContext context)
            : this()
        {
            ColorGrid = (Color[,])info.GetValue("ColorGrid", typeof(Color[,]));
        }
        #endregion

        #region public methods
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("ColorGrid", ColorGrid);
        }

        public bool Equals(Macroblock other)
        {
            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Width == other.Width &&
                Height == other.Height &&
                CompareGrids(ColorGrid, other.ColorGrid);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return Equals(obj as Macroblock);
        }

        public override int GetHashCode()
        {
            return _hashCode.Value;
        }

        public override string ToString()
        {
            return string.Format("Macroblock HashCode: {0}", GetHashCode());
        }

        /// <summary>
        /// Compare two Macroblocks to see if they're similar enough to be considered the same
        /// </summary>
        /// <param name="other">The other macroblock to compare against</param>
        /// <returns>True if the other Macroblock is similar enough to be considered the same. False otherwise</returns>
        public bool IsSimilarTo(Macroblock other)
        {
            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            // Can't compare two macroblocks if they have different dimensions
            if (Height != other.Height || Width != other.Width)
            {
                return false;
            }

            return AreColorsCloseEnough(_averageColor.Value, other._averageColor.Value);
        }
        #endregion

        #region private methods
        private static bool CompareGrids(Color[,] a, Color[,] b)
        {
            int width = a.GetLength(0);
            int height = a.GetLength(1);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color aColor = a[x, y];
                    Color bColor = b[x, y];

                    if (aColor.Equals(bColor) == false)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private int CalculateHashCode()
        {
            int currentHashCode = 5953;
            foreach (var c in ColorGrid)
            {
                currentHashCode = currentHashCode ^ c.GetHashCode();
            }

            return currentHashCode;
        }

        private static bool AreColorsCloseEnough(Color a, Color b)
        {
            int redDiff = Math.Abs(a.R - b.R);
            int greenDiff = Math.Abs(a.G - b.G);
            int blueDiff = Math.Abs(a.B - b.B);

            return redDiff + greenDiff + blueDiff < 45;
        }

        private Color CalculateAverageColor()
        {
            int red, green, blue, runningCount;
            red = green = blue = runningCount = 0;

            foreach (var color in ColorGrid)
            {
                red += color.R;
                green += color.G;
                blue += color.B;

                runningCount++;
            }

            int averageRed = (int)Math.Floor(red / (double)runningCount);
            int averageGreen = (int)Math.Floor(green / (double)runningCount);
            int averageBlue = (int)Math.Floor(blue / (double)runningCount);

            return Color.FromArgb(averageRed, averageGreen, averageBlue);
        }
        #endregion
    }
}
