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
using System.Linq;

namespace Dedup
{
    /// <summary>
    /// Represents an image fingerprint that can be used to compare it against other fingerprints
    /// </summary>
    internal sealed class FingerPrint : IEquatable<FingerPrint>
    {
        private readonly Macroblock _topLeft;
        private readonly Macroblock _topRight;
        private readonly Macroblock _center;
        private readonly Macroblock _bottomLeft;
        private readonly Macroblock _bottomRight;
        private readonly Macroblock _focusSquareTopLeft;
        private readonly Macroblock _focusSquareTopRight;
        private readonly Macroblock _focusSquareBottomLeft;
        private readonly Macroblock _focusSquareBottomRight;

        /// <summary>
        /// Create a new Fingerprint based on the provided Macroblocks
        /// </summary>
        public FingerPrint(
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
            _topLeft = topLeft;
            _topRight = topRight;
            _center = center;
            _bottomLeft = bottomLeft;
            _bottomRight = bottomRight;

            _focusSquareTopLeft = focusSquareBottomLeft;
            _focusSquareTopRight = focusSquareTopRight;
            _focusSquareBottomLeft = focusSquareBottomLeft;
            _focusSquareBottomRight = focusSquareBottomRight;
        }

        public bool Equals(FingerPrint other)
        {
            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Equals(_topLeft, other._topLeft) &&
                Equals(_topRight, other._topRight) &&
                Equals(_center, other._center) &&
                Equals(_bottomLeft, other._bottomLeft) &&
                Equals(_bottomRight, other._bottomRight) &&
                Equals(_focusSquareTopLeft, other._focusSquareTopLeft) &&
                Equals(_focusSquareTopRight, other._focusSquareTopRight) &&
                Equals(_focusSquareBottomLeft, other._focusSquareBottomLeft) &&
                Equals(_focusSquareBottomRight, other._focusSquareBottomRight);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return Equals(obj as FingerPrint);
        }

        public override int GetHashCode()
        {
            return _topLeft.GetHashCode() ^
                _topRight.GetHashCode() ^
                _center.GetHashCode() ^
                _bottomLeft.GetHashCode() ^
                _bottomRight.GetHashCode() ^
                _focusSquareTopLeft.GetHashCode() ^
                _focusSquareTopRight.GetHashCode() ^
                _focusSquareBottomLeft.GetHashCode() ^
                _focusSquareBottomRight.GetHashCode();
        }

        public bool IsSimilarTo(FingerPrint other)
        {
            return _topLeft.IsSimilarTo(other._topLeft) &&
            _topRight.IsSimilarTo(other._topRight) &&
            _center.IsSimilarTo(other._center) &&
            _bottomLeft.IsSimilarTo(other._bottomLeft) &&
            _bottomRight.IsSimilarTo(other._bottomRight) &&
            _focusSquareTopLeft.IsSimilarTo(other._focusSquareTopLeft) &&
            _focusSquareTopRight.IsSimilarTo(other._focusSquareTopRight) &&
            _focusSquareBottomLeft.IsSimilarTo(other._focusSquareBottomLeft) &&
            _focusSquareBottomRight.IsSimilarTo(other._focusSquareBottomRight);
        }
    }
}
