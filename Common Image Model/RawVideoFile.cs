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

using System.IO;
using System.Text;
using System;

namespace CommonImageModel
{
    public sealed class RawVideoFile
    {
        #region private static fields
        private const string RAW_VIDEO_HEADER = "YUV4MPEG2 "; // The space is intentional
        #endregion
        
        #region public properties
        public string Path { get; }
        
        public int Width { get; }
        
        public int Height { get; }
        
        public FPS Framerate { get; }
        #endregion
        
        private RawVideoFile(string path, int width, int height, FPS framerate)
        {
            Path = path;
            Width = width;
            Height = height;
            Framerate = framerate;
        }
        
        public static RawVideoFile DecodeVideoFile(string path)
        {
            if (File.Exists(path) == false)
            {
                throw new ArgumentException("File does not exist");
            }
            
            using (var reader = new BinaryReader(new FileStream(path, FileMode.Open, FileAccess.Read), Encoding.ASCII, false))
            {
                string header = new String(reader.ReadChars(10));
                if (string.Equals(RAW_VIDEO_HEADER, header, StringComparison.OrdinalIgnoreCase) == false)
                {
                    throw new ArgumentException("Incorrect header. File is not a raw video");
                }
                
                throw new Exception();
            }
        }
        
        private static char ReadUntilArgument(BinaryReader reader)
        {
            char c = ' ';
            while (c == ' ')
            {
                c = reader.ReadChar();
            }
            
            return c;
        }
    }
}