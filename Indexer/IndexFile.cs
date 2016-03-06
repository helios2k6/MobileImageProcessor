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
using System.IO;

namespace Indexer
{
    /// <summary>
    /// Represents an index file residing on disk
    /// </summary>
    internal sealed class IndexFile : IEquatable<IndexFile>
    {
        private readonly string _indexFilePath;

        /// <summary>
        /// Construct a new object that represents an index file that may or may not
        /// exist on disk
        /// </summary>
        /// <param name="indexFilePath">The path to the index file</param>
        public IndexFile(string indexFilePath)
        {
            if (string.IsNullOrWhiteSpace(indexFilePath))
            {
                throw new ArgumentException();
            }

            _indexFilePath = indexFilePath;
        }

        /// <summary>
        /// The path to the index file path
        /// </summary>
        public string Path
        {
            get { return _indexFilePath; }
        }

        // override object.Equals
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return Equals(obj as IndexFile);
        }

        public override int GetHashCode()
        {
            return Path.GetHashCode();
        }

        public bool Equals(IndexFile other)
        {
            if (other == null || GetType() != other.GetType())
            {
                return false;
            }

            if (this == other)
            {
                return true;
            }

            return string.Equals(
                Path,
                other.Path,
                StringComparison.Ordinal
            );
        }

        public override string ToString()
        {
            return Path;
        }
    }
}
