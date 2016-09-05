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
    public sealed class FastBoxBlurGaussianBlurTransformation : ITransformation, IDisposable, IBoxBlur
    {
        private bool _disposed;

        private readonly Image _sourceImage;
        private readonly int _radius;

        public FastBoxBlurGaussianBlurTransformation(Image sourceImage, int radius)
        {
            _sourceImage = sourceImage;
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
                using (var sourceLockbitImage = new WritableLockBitImage(_sourceImage))
                using (var outputLockbitImage = new WritableLockBitImage(_sourceImage.Width, _sourceImage.Height))
                {
                    // Get boxes
                    int index = 0;
                    foreach (int boxWidth in this.EnumerateBoxesForGauss(_radius, 3))
                    {
                        if (index % 2 == 0)
                        {
                            PerformSplitBoxBlur(sourceLockbitImage, outputLockbitImage, (boxWidth - 1) / 2);
                        }
                        else
                        {
                            PerformSplitBoxBlur(outputLockbitImage, sourceLockbitImage, (boxWidth - 1) / 2);
                        }
                        index++;
                    }

                    outputLockbitImage.Lock();
                    return outputLockbitImage.GetImage().ToMaybe();
                }
            }
            catch (Exception)
            {
            }

            return Maybe<Image>.Nothing;
        }

        private void PerformSplitBoxBlur(WritableLockBitImage sourceImage, WritableLockBitImage outputImage, int radius)
        {
            // Copy pixels from source to output
            this.CopyPixels(sourceImage, outputImage);

            // The switching of outputImage and souceImage is INTENTIONAL!
            PerformHorizontalBoxBlur(outputImage, sourceImage, radius);
            PerformTotalBoxBlur(sourceImage, outputImage, radius);
        }

        private static void PerformHorizontalBoxBlur(WritableLockBitImage sourceImage, WritableLockBitImage outputImage, int radius)
        {
            double valueDivisor = radius + radius + 1;
            for (int row = 0; row < sourceImage.Height; row++)
            {
                for (int col = 0; col < sourceImage.Width; col++)
                {
                    int totalValueRed = 0, totalValueGreen = 0, totalValueBlue = 0;
                    for (int colIndex = col - radius; colIndex < col + radius + 1; colIndex++)
                    {
                        int chosenCol = Math.Min(sourceImage.Width - 1, Math.Max(0, colIndex));
                        Color chosenPixel = sourceImage.GetPixel(chosenCol, row);
                        totalValueRed += chosenPixel.R;
                        totalValueGreen += chosenPixel.G;
                        totalValueBlue += chosenPixel.B;
                    }
                    outputImage.SetPixel(col, row, Color.FromArgb(
                        (int)Math.Round(totalValueRed / valueDivisor),
                        (int)Math.Round(totalValueGreen / valueDivisor),
                        (int)Math.Round(totalValueBlue / valueDivisor)
                    ));
                }
            }
        }

        private static void PerformTotalBoxBlur(WritableLockBitImage sourceImage, WritableLockBitImage outputImage, int radius)
        {
            double valueDivisor = radius + radius + 1;
            for (int row = 0; row < sourceImage.Height; row++)
            {
                for (int col = 0; col < sourceImage.Width; col++)
                {
                    int totalValueRed = 0, totalValueGreen = 0, totalValueBlue = 0;
                    for (int rowIndex = row - radius; rowIndex < row + radius + 1; rowIndex++)
                    {
                        int chosenRowIndex = Math.Min(sourceImage.Height - 1, Math.Max(0, rowIndex));
                        Color chosenPixel = sourceImage.GetPixel(col, chosenRowIndex);
                        totalValueRed += chosenPixel.R;
                        totalValueGreen += chosenPixel.G;
                        totalValueBlue += chosenPixel.B;
                    }

                    outputImage.SetPixel(col, row, Color.FromArgb(
                        (int)Math.Round(totalValueRed / valueDivisor),
                        (int)Math.Round(totalValueGreen / valueDivisor),
                        (int)Math.Round(totalValueBlue / valueDivisor)
                    ));
                }
            }
        }
    }
}