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

namespace CommonImageModel
{
    /// <summary>
    /// Represents the frames per second of a media file as a rational number
    /// </summary>
    public sealed class FPS : IEquatable<FPS>, IComparable<FPS>
    {
        #region ctor
        /// <summary>
        /// Construct a default FPS of 0/1, avoiding the non-determinate form
        /// of 0/0
        /// </summary>
        public FPS()
        {
            Numerator = 0;
            Denominator = 1;
        }

        /// <summary>
        /// Construct an FPS given the numerator and denominator
        /// </summary>
        public FPS(int numerator, int denominator)
        {
            if (numerator < 0 || denominator < 1)
            {
                throw new ArgumentException("Numerator must be greater than or equal to 0 and denominator must be strictly greater than 0");
            }
            Numerator = numerator;
            Denominator = denominator;
        }
        #endregion

        #region public properties
        /// <summary>
        /// The FPS numerator
        /// </summary>
        public int Numerator { get; }

        /// <summary>
        /// The FPS denominator
        /// </summary>
        public int Denominator { get; }
        #endregion

        #region public methods
        public override bool Equals(object other)
        {
            return Equals(other as FPS);
        }

        public override int GetHashCode()
        {
            return Numerator.GetHashCode() ^
                Denominator.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("FPS: {0}/{1}", Numerator, Denominator);
        }

        public bool Equals(FPS other)
        {
            if (EqualsPreamble(other) == false)
            {
                return false;
            }

            return Equals(Numerator, other.Numerator) &&
                Equals(Denominator, other.Denominator);
        }

        public int CompareTo(FPS other)
        {
            int ourMultipliedNumerator = Numerator * other.Denominator;
            int otherMultipliedNumerator = other.Numerator * Denominator;

            return ourMultipliedNumerator.CompareTo(otherMultipliedNumerator);
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
        #endregion
    }
}