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

namespace CommonImageModel.Y4M
{
    /// <summary>
    /// Decodes a file saved in the Y4M Format
    /// </summary>
    public sealed class Y4MDecoder
    {
        #region private fields
        private readonly Lazy<Y4MFile> _decodedFile;
        #endregion

        #region public properties
        /// <summary>
        /// Gets the path to the Y4M file 
        /// </summary>
        public string File { get; }

        /// <summary>
        /// Gets the decoded Y4M file
        /// </summary>
        public Y4MFile Y4MFile
        {
            get { return _decodedFile.Value; }
        }
        #endregion

        #region ctor
        /// <summary>
        /// Construct a new Y4MDecoder object
        /// </summary>
        /// <param name="file">The path to the Y4M File</param>
        public Y4MDecoder(string file)
        {
            File = file;
            _decodedFile = new Lazy<Y4MFile>(() => DecodeFile(File));
        }
        #endregion

        #region private methods
        private static Y4MFile DecodeFile(string filePath)
        {
            return null;
        }
        #endregion
    }
}
