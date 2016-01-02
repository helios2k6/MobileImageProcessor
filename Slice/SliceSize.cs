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

namespace Slice
{
    /// <summary>
    /// Represents the size of a slice
    /// </summary>
    internal sealed class SliceSize : IEquatable<SliceSize>
    {
        public SliceSize(int width, int height, int x, int y)
        {
            Size = new Size(width, height);
            Point = new Point(x, y);
        }

        /// <summary>
        /// The SliceSize for a snapshot from an iPad
        /// </summary>
        public static readonly SliceSize IPadSliceSize = new SliceSize(274, 65, 0, 40);

        /// <summary>
        /// The SliceSize for a snapshot from an iPhone
        /// </summary>
        public static readonly SliceSize IPhoneSliceSize = new SliceSize(240, 62, 0, 40);

        /// <summary>
        /// The size of the viewport rectangle
        /// </summary>
        public Size Size { get; private set; }

        /// <summary>
        /// The upper left-hand corner of the viewport rectangle
        /// </summary>
        public Point Point { get; private set; }

        public bool Equals(SliceSize other)
        {
            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Equals(Size, other.Size) &&
                Equals(Point, other.Point);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return Equals(obj as SliceSize);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
