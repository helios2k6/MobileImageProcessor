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
using CommonImageModel;

namespace Match
{
    /// <summary>
    /// Calculates the similarity between two images
    /// </summary>
    internal static class SimilarityCalculator
    {
        /// <summary>
        /// Calculate the similarity between two images
        /// </summary>
        /// <remarks>
        /// Images that are not the same size will always return a result of 0, as they cannot 
        /// be the same.
        /// </remarks>
        /// <param name="original">The original image to compare against</param>
        /// <param name="candidate">The candidate image to compare with the original</param>
        /// <returns>An integer, from 0-100 indicating how similar the two images are</returns>
        public static int CalculateSimilarityIndex(ImageWrapper original, ImageWrapper candidate)
        {
            // Images that are not the same size are immediately discounted
            if (original.Image.Width != candidate.Image.Width || original.Image.Height != candidate.Image.Height)
            {
                return 0;
            }

            return (int)(SSIMCalculator.Compute(original.Image, candidate.Image) * 100);
        }
    }
}
