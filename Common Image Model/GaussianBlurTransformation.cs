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
using Functional.Maybe;

namespace CommonImageModel
{
    public sealed class GaussianBlurTransformation : ITransformation, IDisposable
    {
        private const double GAUSSIAN_RADIUS_RANGE = 2.57;

        private bool _disposed;
        private readonly LockBitImage _sourceImage;
        private readonly int _radius;

        public GaussianBlurTransformation(Image sourceImage, int radius)
        {
            _disposed = false;
            _sourceImage = new LockBitImage(sourceImage);
            _radius = radius;
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }
            _disposed = true;
            _sourceImage.Dispose();
        }

        public Maybe<Image> TryTransform()
        {
            try
            {
                // Output color matrix
                using (var outputBitmap = new WritableLockBitImage(_sourceImage.Width, _sourceImage.Height))
                {
                    // Setting up some constants up here
                    double exponentDenominator = 2 * _radius * _radius;
                    double gaussianWeightDenominator = exponentDenominator * Math.PI;

                    int radiusEffectiveRange = (int)Math.Ceiling(_radius * GAUSSIAN_RADIUS_RANGE);
                    // Go through every single pixel
                    for (int row = 0; row < _sourceImage.Height; row++)
                    {
                        for (int col = 0; col < _sourceImage.Width; col++) 
                        {
                            // Calculate the weighted sum of all of the neighboring pixels, 
                            // governed by the radiusEffectiveRange variable above, and take
                            // the average value and then set it to the value of whatever
                            // pixel is at (row, col) 
                            double neighborhoodRedPixelWeightedSum = 0,
                                neighborhoodGreenPixelWeightedSum = 0,
                                neighborhoodBluePixelWeightedSum = 0,
                                sumOfGaussianValues = 0;

                            for (
                                int neighboringPixelRow = row - radiusEffectiveRange;
                                neighboringPixelRow <= row + radiusEffectiveRange;
                                neighboringPixelRow++
                            )
                            {
                                for (
                                    int neighboringPixelCol = col - radiusEffectiveRange;
                                    neighboringPixelCol <= col + radiusEffectiveRange;
                                    neighboringPixelCol++
                                )
                                {
                                    // This is used so we don't try to get a pixel that's outside the boundaries
                                    // of the image (e.g. (-1, 0))
                                    int chosenRow = Math.Min(_sourceImage.Height - 1, Math.Max(0, neighboringPixelRow));
                                    int chosenCol = Math.Min(_sourceImage.Width - 1, Math.Max(0, neighboringPixelCol));

                                    // The Gaussian Formula is: (e ^ ((x^2 + y^2) / 2 * radius^2)) / (2 * PI * radius^2)
                                    // Here, x = col and y = row. We have to subtract the neighboringPixelCol/Row from
                                    // col/row so that we can translate the coordinate back to the origin. This is because
                                    // the gaussian function is expressed as a function from the distance from the origin
                                    double exponentNumerator = ((neighboringPixelCol - col) * (neighboringPixelCol - col)) + 
                                        ((neighboringPixelRow - row) * (neighboringPixelRow - row));
                                    
                                    double gaussianWeight = Math.Exp(-exponentNumerator / exponentDenominator)
                                        / gaussianWeightDenominator;
                                    
                                    Color currentPixel = _sourceImage.GetPixel(chosenCol, chosenRow);
                                    neighborhoodRedPixelWeightedSum += currentPixel.R * gaussianWeight;
                                    neighborhoodGreenPixelWeightedSum += currentPixel.G * gaussianWeight;
                                    neighborhoodBluePixelWeightedSum += currentPixel.B * gaussianWeight;

                                    sumOfGaussianValues += gaussianWeight;
                                }
                            }
                            outputBitmap.SetPixel(col, row, Color.FromArgb(
                                (int)Math.Round(neighborhoodRedPixelWeightedSum / sumOfGaussianValues),
                                (int)Math.Round(neighborhoodGreenPixelWeightedSum / sumOfGaussianValues),
                                (int)Math.Round(neighborhoodBluePixelWeightedSum / sumOfGaussianValues)
                            ));
                        }
                    }
                    outputBitmap.Lock();
                    return outputBitmap.GetImage().ToMaybe<Image>();
                }
            }
            catch (Exception)
            {
            }

            return Maybe<Image>.Nothing;
        }
    }
}