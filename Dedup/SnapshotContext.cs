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
using System;
using System.IO;

namespace Dedup
{
    /// <summary>
    /// Represents the context around a Snapshot
    /// </summary>
    internal sealed class SnapshotContext : IDisposable
    {
        /// <summary>
        /// Construct a new Snapshot Context
        /// </summary>
        /// <param name="snapshotPath"></param>
        /// <param name="scaledDownSnapshot"></param>
        public SnapshotContext(
            string snapshotPath,
            LockBitImage scaledDownSnapshot,
            ImageFingerPrint fingerPrint
        )
        {
            SnapshotPath = snapshotPath;
            ScaledDownSnapshot = scaledDownSnapshot;
            FingerPrint = fingerPrint;
        }

        /// <summary>
        /// The path to the snapshot file
        /// </summary>
        public string SnapshotPath { get; private set; }

        /// <summary>
        /// The scaled down snapshot
        /// </summary>
        public LockBitImage ScaledDownSnapshot { get; private set; }

        /// <summary>
        /// The snapshot fingerprint 
        /// </summary>
        public ImageFingerPrint FingerPrint { get; private set; }

        /// <summary>
        /// Dispose of this object
        /// </summary>
        public void Dispose()
        {
            ScaledDownSnapshot.Dispose();
        }

        public override string ToString()
        {
            return Path.GetFileName(SnapshotPath);
        }
    }
}
