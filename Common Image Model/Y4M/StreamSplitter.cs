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
using System.Collections.Generic;
using System.IO;

namespace CommonImageModel.Y4M
{
    /// <summary>
    /// Splits the raw byte stream of a Y4M file
    /// </summary>
    public sealed class StreamSplitter
    {
        private readonly char[] FileSignature = new[] {'Y', 'U', 'V', '4', 'M', 'P', 'E', 'G', '2', ' '};
        
        public IEnumerable<byte> VideoHeaderStream { get; }
        
        public IEnumerable<IEnumerable<byte>> FrameByteStreams { get; }
        
        private StreamSplitter(IEnumerable<byte> videoHeaderStream, IEnumerable<IEnumerable<byte>> frameByteStreams)
        {
            VideoHeaderStream = videoHeaderStream;
            FrameByteStreams = frameByteStreams;
        }
        
        public static StreamSplitter GenerateByteStreamFromFile(string pathToFile)
        {
            using (var fileStream = new FileStream(pathToFile, FileMode.Open, FileAccess.Read))
            using (var binaryReader = new BinaryReader(fileStream))
            {
                
            }
            throw new NotImplementedException();
        }
    }
}