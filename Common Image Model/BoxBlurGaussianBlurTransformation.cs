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
    public sealed class BoxBlurGaussianBlurTransformation : ITransformation, IDisposable, IBoxBlur
    {
        private bool _disposed;
        private readonly Image _sourceImage;
        private readonly int _radius;

        public BoxBlurGaussianBlurTransformation(Image sourceImage, int radius)
        {
            _sourceImage = sourceImage.Clone() as Image;
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
                using (var sourceImage = new WritableLockBitImage(_sourceImage))
                using (var outputLockbitImage = new WritableLockBitImage(_sourceImage.Width, _sourceImage.Height))
                {
                    int index = 0;
                    foreach (int boxWidth in this.EnumerateBoxesForGauss(_radius, 3))
                    {
                        if (index % 2 == 0)
                        {
                            BoxBlurForGaussianBlurHelper(sourceImage, outputLockbitImage, (boxWidth - 1) / 2);
                        }
                        else
                        {
                            BoxBlurForGaussianBlurHelper(outputLockbitImage, sourceImage, (boxWidth - 1) / 2);
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

        private static void BoxBlurForGaussianBlurHelper(WritableLockBitImage inputImage, WritableLockBitImage outputImage, int radius)
        {
            double radiusDivisor = (radius + radius + 1) * (radius + radius + 1);
            for (int row = 0; row < inputImage.Height; row++)
            {
                for (int col = 0; col < inputImage.Width; col++)
                {
                    int cumulativeSourceRedValue = 0,
                        cumulativeSourceGreenValue = 0,
                        cumulativeSourceBlueValue = 0;
                    for (int rowIndex = row - radius; rowIndex < row + radius + 1; rowIndex++)
                    {
                        for (int colIndex = col - radius; colIndex < col + radius + 1; colIndex++)
                        {
                            int chosenRow = Math.Min(inputImage.Height - 1, Math.Max(0, rowIndex));
                            int chosenCol = Math.Min(inputImage.Width - 1, Math.Max(0, colIndex));

                            Color chosenPixel = inputImage.GetPixel(chosenCol, chosenRow);
                            cumulativeSourceRedValue += chosenPixel.R;
                            cumulativeSourceGreenValue += chosenPixel.G;
                            cumulativeSourceBlueValue += chosenPixel.B;
                        }
                    }

                    Color bluredColor = Color.FromArgb(
                        (int)Math.Round(cumulativeSourceRedValue / radiusDivisor),
                        (int)Math.Round(cumulativeSourceGreenValue / radiusDivisor),
                        (int)Math.Round(cumulativeSourceBlueValue / radiusDivisor)
                    );
                    outputImage.SetPixel(col, row, bluredColor);
                }
            }
        }
    }
}