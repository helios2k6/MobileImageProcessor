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

using System;
using System.Drawing;

namespace Dedup
{
    /// <summary>
    /// Represents a contiguous grid of colors
    /// </summary>
    internal sealed class Macroblock : IEquatable<Macroblock>
    {
        private readonly Color[,] _colorGrid;
        private readonly int _width;
        private readonly int _height;
        private readonly Lazy<int> _hashCode;
        private readonly Lazy<Color> _averageColor;

        /// <summary>
        /// Create a fingerprint with the provided macroblock
        /// </summary>
        /// <param name="colorGrid"></param>
        public Macroblock(Color[,] colorGrid)
        {
            _colorGrid = colorGrid;
            _width = colorGrid.GetLength(0);
            _height = colorGrid.GetLength(1);
            _hashCode = new Lazy<int>(() => CalculateHashCode(_colorGrid));
            _averageColor = new Lazy<Color>(() => CalculateAverageColor(_colorGrid));
        }

        public int Width
        {
            get { return _width; }
        }

        public int Height
        {
            get { return _height; }
        }

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
                CompareGrids(_colorGrid, other._colorGrid);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return Equals(obj as Macroblock);
        }

        private static int CalculateHashCode(Color[,] colorGrid)
        {
            int currentHashCode = 0;
            foreach (var c in colorGrid)
            {
                currentHashCode = currentHashCode ^ c.GetHashCode();
            }

            return currentHashCode;
        }

        public override int GetHashCode()
        {
            return _hashCode.Value;
        }

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

        private static bool AreColorsCloseEnough(Color a, Color b)
        {
            int redDiff = Math.Abs(a.R - b.R);
            int greenDiff = Math.Abs(a.G - b.G);
            int blueDiff = Math.Abs(a.B - b.B);

            return redDiff + greenDiff + blueDiff < 45;
        }

        private static Color CalculateAverageColor(Color[,] colorGrid)
        {
            int red, green, blue, runningCount;
            red = green = blue = runningCount = 0;

            foreach (var color in colorGrid)
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
    }
}
