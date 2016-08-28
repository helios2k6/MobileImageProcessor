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
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace CommonImageModel
{
    /// <summary>
    /// An Image object that allows you to read from it form multiple threads
    /// </summary>
    public sealed class LockBitImage : IDisposable, IImageFrame
    {
        private readonly Image _image;
        private readonly Bitmap _bitmap;
        private readonly BitmapData _bitmapData;
        private readonly byte[] _buffer;
        private readonly int _bitDepth;
        private readonly int _width;
        private readonly int _height;

        /// <summary>
        /// Creates a new LockBitImage from the Image passed in
        /// </summary>
        /// <remarks>
        /// The reference to the Image passed in is never kept around. A clone of the Image 
        /// is created and the original reference is discarded
        /// </remarks>
        /// <param name="image">The image to create a Lockbit image of</param>
        public LockBitImage(Image image)
        {
            _image = image.Clone() as Image;
            _bitmap = new Bitmap(_image);
            _bitmapData = _bitmap.LockBits(
                new Rectangle(0, 0, image.Width, image.Height),
                ImageLockMode.ReadOnly,
                _image.PixelFormat
            );

            _bitDepth = Image.GetPixelFormatSize(_image.PixelFormat);
            if (_bitDepth != 8 && _bitDepth != 24 && _bitDepth != 32)
            {
                throw new ArgumentException("Only 8, 24, and 32 bit pixels are supported.");
            }
            _buffer = new byte[_bitmapData.Width * _bitmapData.Height * (_bitDepth / 8)];
            _width = _image.Width;
            _height = _image.Height;

            Marshal.Copy(_bitmapData.Scan0, _buffer, 0, _buffer.Length);
        }

        /// <summary>
        /// Get the pixel at a specific point in this image
        /// </summary>
        /// <param name="x">The X coordinate</param>
        /// <param name="y">The Y coordinate</param>
        /// <returns>A color representing this pixel</returns>
        public Color GetPixel(int x, int y)
        {
            Color clr = Color.Empty;

            // Get color components count
            int cCount = _bitDepth / 8;

            // Get start index of the specified pixel
            int offset = ((y * Width) + x) * cCount;

            if (offset > _buffer.Length - cCount)
            {
                throw new IndexOutOfRangeException();
            }

            if (_bitDepth == 32) // For 32 bpp get Red, Green, Blue and Alpha
            {
                byte b = _buffer[offset];
                byte g = _buffer[offset + 1];
                byte r = _buffer[offset + 2];
                byte a = _buffer[offset + 3]; // a
                clr = Color.FromArgb(a, r, g, b);
            }
            if (_bitDepth == 24) // For 24 bpp get Red, Green and Blue
            {
                byte b = _buffer[offset];
                byte g = _buffer[offset + 1];
                byte r = _buffer[offset + 2];
                clr = Color.FromArgb(r, g, b);
            }
            if (_bitDepth == 8)  // For 8 bpp get color value (Red, Green and Blue values are the same)
            {
                byte c = _buffer[offset];
                clr = Color.FromArgb(c, c, c);
            }
            return clr;
        }

        /// <summary>
        /// The width of the image
        /// </summary>
        public int Width
        {
            get { return _width; }
        }

        /// <summary>
        /// The height of the image
        /// </summary>
        public int Height
        {
            get { return _height; }
        }

        /// <summary>
        /// Clones the backing image and returns it
        /// </summary>
        /// <returns>A clone of the backing Image</returns>
        public Image GetImageClone()
        {
            return _image.Clone() as Image;
        }

        public void Dispose()
        {
            _bitmap.UnlockBits(_bitmapData);
            _image.Dispose();
            _bitmap.Dispose();
        }
    }
}
