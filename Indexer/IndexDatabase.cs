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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace Indexer
{
    /// <summary>
    /// Wraps the IndexFile and allows you to perform queries via optimized mappings
    /// on the index entries as well as transations on the index file
    /// </summary>
    internal sealed class IndexDatabase
    {
        #region public enum
        public enum SerializationMethod
        {
            JSON,
            BINARY,
        }
        #endregion

        #region private fields
        private readonly string _pathToIndexFile;
        private readonly ConcurrentQueue<IndexEntry> _queuedIndexEntriesForAddition;
        private readonly SerializationMethod _serializationMethod;

        private IndexEntries _indexEntries;
        private Lazy<IDictionary<ImageFingerPrint, ICollection<IndexEntry>>> _hashToEntriesMap;
        #endregion

        #region ctor
        /// <summary>
        /// Initialize an IndexDatabase with the path to the index file,
        /// whether it exists or not
        /// </summary>
        /// <param name="pathToIndexFile">The path to the index file</param>
        /// <param name="method">The serialization method to use</param>
        /// <remarks>
        /// If the file exists, this object will attempt to deserialize it. If it does
        /// not exist, a new file will be created
        /// </remarks>
        public IndexDatabase(string pathToIndexFile, SerializationMethod method)
            : this()
        {
            _pathToIndexFile = pathToIndexFile;
            _indexEntries = DeserializeIndexDatabase(pathToIndexFile, method);
            _serializationMethod = method;
        }

        private IndexDatabase()
        {
            _hashToEntriesMap =
                new Lazy<IDictionary<ImageFingerPrint, ICollection<IndexEntry>>>(IndexAllEntries);
            _queuedIndexEntriesForAddition = new ConcurrentQueue<IndexEntry>();
            _serializationMethod = SerializationMethod.JSON;
        }
        #endregion

        #region public methods
        /// <summary>
        /// Attempts to find the set of entries that match the given fingerprint
        /// </summary>
        /// <param name="fingerPrint">The image fingerprint to look for</param>
        public IEnumerable<IndexEntry> TryFindEntries(ImageFingerPrint fingerPrint)
        {
            // TODO: Develop locality-based hashing data-structure
            return from kvp in _hashToEntriesMap.Value
                   where kvp.Key.IsSimilarTo(fingerPrint)
                   from entry in kvp.Value
                   select entry;
        }

        /// <summary>
        /// Queue an entry to be added to the database. In order to completely serialize
        /// the entry to disk, you must call Flush()
        /// </summary>
        public void QueueAddEntry(IndexEntry entry)
        {
            _queuedIndexEntriesForAddition.Enqueue(entry);
        }

        /// <summary>
        /// Flush any queued entries for addition to disk
        /// </summary>
        public void Flush()
        {
            var updatedIndexEntriesObject = new IndexEntries
            {
                Entries = _queuedIndexEntriesForAddition.ToArray(),
            };

            if (_serializationMethod == SerializationMethod.JSON)
            {
                SerializeNewEntriesJson(_pathToIndexFile, JsonConvert.SerializeObject(updatedIndexEntriesObject));
            }
            else
            {
                SerializeNewEntriesBinary(_pathToIndexFile, updatedIndexEntriesObject);
            }

            _indexEntries = updatedIndexEntriesObject;
            ClearQueuedEntries();
            _hashToEntriesMap = new Lazy<IDictionary<ImageFingerPrint, ICollection<IndexEntry>>>(IndexAllEntries);
        }
        #endregion

        #region private methods
        private static IndexEntries DeserializeIndexDatabase(string pathToIndexFile, SerializationMethod method)
        {
            if (File.Exists(pathToIndexFile) == false)
            {
                return new IndexEntries();
            }

            switch (method)
            {
                case SerializationMethod.BINARY:
                    using (var stream = new FileStream(pathToIndexFile, FileMode.Open, FileAccess.Read, FileShare.None))
                    {
                        var formatter = new BinaryFormatter();
                        return (IndexEntries)formatter.Deserialize(stream);
                    }
                case SerializationMethod.JSON:
                    return JsonConvert.DeserializeObject<IndexEntries>(File.ReadAllText(pathToIndexFile));
            }

            throw new InvalidOperationException("Did not find appropriate deserialization method");
        }

        private void ClearQueuedEntries()
        {
            IndexEntry _;
            while (_queuedIndexEntriesForAddition.TryDequeue(out _))
            {
                // Do nothing
            }
        }

        private static void SerializeNewEntries(string pathToIndexFile, Action writeToFileAction)
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
                writeToFileAction.Invoke();

                // Attempt to delete original index file
                if (File.Exists(movedOriginalFilePath))
                {
                    File.Delete(movedOriginalFilePath);
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Unable to write index file. {0}", e);

                // Attempt to roll back and move the original index file back
                if (movedOriginalFilePath != null && File.Exists(movedOriginalFilePath))
                {
                    File.Move(movedOriginalFilePath, pathToIndexFile);
                }

                throw e;
            }
        }

        private static void SerializeNewEntriesBinary(string pathToIndexFile, IndexEntries entries)
        {
            SerializeNewEntries(pathToIndexFile, () =>
            {
                using (var stream = new FileStream(pathToIndexFile, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    var formatter = new BinaryFormatter();
                    formatter.Serialize(stream, entries);
                    stream.Close();
                }
            });
        }

        private static void SerializeNewEntriesJson(string pathToIndexFile, string jsonBlob)
        {
            SerializeNewEntries(pathToIndexFile, () => File.WriteAllText(pathToIndexFile, jsonBlob));
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
        #endregion
    }
}
