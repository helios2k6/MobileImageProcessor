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
namespace Slice
{
    /// <summary>
    /// Represents the size of a slice
    /// </summary>
    public sealed class SliceSize : IEquatable<SliceSize>
    {
        /// <summary>
        /// The SliceSize for a snapshot from an iPad
        /// </summary>
        public static readonly SliceSize IPadSliceSize = SliceSize.Create(274, 65, 0, 40);

        /// <summary>
        /// The SliceSize for a snapshot from an iPhone
        /// </summary>
        public static readonly SliceSize IPhoneSliceSize = SliceSize.Create(240, 62, 0, 40);

        /// <summary>
        /// Represents the width of the slice
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// Represents the Height of the slice
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// Represents the offset from the top of the 
        /// image at which the slice begins
        /// </summary>
        public int YOffset { get; private set; }

        /// <summary>
        /// Represents the offset from the left of the
        /// image at which the slice begins
        /// </summary>
        public int XOffset { get; private set; }

        /// <summary>
        /// Factory method for creating a SliceSize
        /// </summary>
        /// <param name="width">The width of the slice</param>
        /// <param name="height">The height of the slice</param>
        /// <param name="xOffset">the x offset</param>
        /// <param name="yOffset">The y offset</param>
        /// <returns>A new SliceSize</returns>
        public static SliceSize Create(
            int width,
            int height,
            int xOffset,
            int yOffset
        )
        {
            return new SliceSize
            {
                Width = width,
                Height = height,
                YOffset = yOffset,
                XOffset = xOffset,
            };
        }

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

            return Width == other.Width &&
                Height == other.Height &&
                YOffset == other.YOffset &&
                XOffset == other.XOffset;
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
