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
    public sealed class CropTransformation : ITransformation, IDisposable
    {
        private bool _disposed;
        private readonly Image _sourceImage;
        private readonly Point _point;
        private readonly Size _size;

        public CropTransformation(Image sourceImage, Point point, Size size)
        {
            _disposed = false;
            _sourceImage = sourceImage.Clone() as Image;
            _point = point;
            _size = size;
        }

        public Maybe<Image> TryTransform()
        {
            try
            {
                if (_point.X == 0 && _point.Y == 0 && _size.Width == _sourceImage.Width && _size.Height == _sourceImage.Height)
                {
                    return (_sourceImage.Clone() as Image).ToMaybe();
                }

                using (var bitmap = new Bitmap(_sourceImage))
                {
                    return (bitmap.Clone(new Rectangle(_point, _size), bitmap.PixelFormat) as Image).ToMaybe();
                }
            }
            catch(Exception)
            {
            }

            return Maybe<Image>.Nothing;
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
    }
}