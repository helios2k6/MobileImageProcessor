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
        private readonly string _pathToIndexFile;
        private readonly IndexEntries _indexEntries;
        private readonly Lazy<IDictionary<ImageFingerPrint, ICollection<IndexEntry>>> _hashToEntriesMap;
        private readonly List<IndexEntry> _queuedIndexEntriesForAddition;

        private IndexDatabase()
        {
            _hashToEntriesMap =
                new Lazy<IDictionary<ImageFingerPrint, ICollection<IndexEntry>>>(IndexAllEntries);
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
            _pathToIndexFile = pathToIndexFile;
            _indexEntries = File.Exists(_pathToIndexFile)
                ? JsonConvert.DeserializeObject<IndexEntries>(File.ReadAllText(_pathToIndexFile))
                : new IndexEntries();
        }

        /// <summary>
        /// Attempts to find the set of entries that match the given fingerprint
        /// </summary>
        /// <param name="fingerPrint">The image fingerprint to look for</param>
        public Maybe<IEnumerable<IndexEntry>> TryFindEntries(ImageFingerPrint fingerPrint)
        {
            ICollection<IndexEntry> bucket;
            if (_hashToEntriesMap.Value.TryGetValue(fingerPrint, out bucket))
            {
                return bucket.ToMaybe<IEnumerable<IndexEntry>>();
            }

            return Maybe<IEnumerable<IndexEntry>>.Nothing;
        }

        /// <summary>
        /// Queue an entry to be added to the database. In order to completely serialize
        /// the entry to disk, you must call Flush()
        /// </summary>
        public void QueueAddEntry(IndexEntry entry)
        {
            _queuedIndexEntriesForAddition.Add(entry);
        }

        /// <summary>
        /// Flush any queued entries for addition to disk
        /// </summary>
        public void Flush()
        {
            var freshEntries = new List<IndexEntry>(_indexEntries.Entries);
            freshEntries.AddRange(_queuedIndexEntriesForAddition);

            var updatedIndexEntriesObject = new IndexEntries
            {
                Entries = freshEntries.ToArray(),
            };

            var serializedEntriesFile = JsonConvert.SerializeObject(updatedIndexEntriesObject);

            // Rename old file first
            if (TrySerializeNewEntries(_pathToIndexFile, serializedEntriesFile) == false)
            {
                throw new InvalidOperationException("Unable to serialize index file");
            }
            
             _queuedIndexEntriesForAddition.Clear();
        }

        private static bool TrySerializeNewEntries(string pathToIndexFile, string jsonBlob)
        {
            string movedOriginalFilePath = null;
            try
            {
                movedOriginalFilePath = Path.Combine(
                    Path.GetDirectoryName(pathToIndexFile),
                    string.Format(
                        "{0}_temp_move{1}",
                        Path.GetFileNameWithoutExtension(pathToIndexFile),
                        Path.GetExtension(pathToIndexFile)
                    )
                );
                if (File.Exists(pathToIndexFile))
                {
                    // Attempt to move the file
                    File.Move(pathToIndexFile, movedOriginalFilePath);
                }

                // Attempt to write the new file to 
                File.WriteAllText(pathToIndexFile, jsonBlob);

                // Attempt to delete original index file
                if (File.Exists(movedOriginalFilePath))
                {
                    File.Delete(movedOriginalFilePath);
                }

                return true;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Unable to write index file. {0}", e);

                // Attempt to roll back and move the original index file back
                if (movedOriginalFilePath != null && File.Exists(movedOriginalFilePath))
                {
                    File.Move(movedOriginalFilePath, pathToIndexFile);
                }
            }

            return false;
        }

        private IDictionary<ImageFingerPrint, ICollection<IndexEntry>> IndexAllEntries()
        {
            var indexMap = new Dictionary<ImageFingerPrint, ICollection<IndexEntry>>();
            foreach (var entry in _indexEntries.Entries)
            {
                ICollection<IndexEntry> entryBucket;
                if (indexMap.TryGetValue(entry.FrameHash, out entryBucket) == false)
                {
                    entryBucket = new HashSet<IndexEntry>();
                    indexMap.Add(entry.FrameHash, entryBucket);
                }

                entryBucket.Add(entry);
            }

            return indexMap;
        }
    }
}
