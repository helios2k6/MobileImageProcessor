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
using Functional.Maybe;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Indexer
{
    /// <summary>
    /// Wraps the IndexFile and allows you to perform queries via optimized mappings
    /// on the index entries as well as transations on the index file
    /// </summary>
    internal sealed class IndexDatabase
    {
        private readonly IndexEntries _indexFile;
        private readonly Lazy<IDictionary<ImageFingerPrint, ICollection<IndexEntry>>> _hashToEntriesMap;

        private IndexDatabase()
        {
            _hashToEntriesMap = new Lazy<IDictionary<ImageFingerPrint, ICollection<IndexEntry>>>(IndexAllEntries);
        }

        /// <summary>
        /// The deserialized index file
        /// </summary>
        /// <param name="indexFile">The index entries</param>
        public IndexDatabase(IndexEntries indexFile)
            : this()
        {
            _indexFile = indexFile;
        }

        /// <summary>
        /// Initialize an IndexDatabase with the path to the index file,
        /// whether it exists or not
        /// </summary>
        /// <param name="pathToIndexFile">The path to the index file</param>
        /// <remarks>
        /// If the file exists, this object will attempt to deserialize it. If it does
        /// not exist, a new file will be created
        /// </remarks>
        public IndexDatabase(string pathToIndexFile)
            : this()
        {
            _indexFile = File.Exists(pathToIndexFile)
                ? JsonConvert.DeserializeObject<IndexEntries>(File.ReadAllText(pathToIndexFile))
                : new IndexEntries();
        }

        private IDictionary<ImageFingerPrint, ICollection<IndexEntry>> IndexAllEntries()
        {
            throw new NotImplementedException();
        }
    }
}
