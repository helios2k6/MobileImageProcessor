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
using System.Linq;

namespace Dedup
{
    /// <summary>
    /// Class for detecting duplicate images or images that are similar enough
    /// </summary>
    internal static class DuplicateSnapshotGrouper
    {
        private const int FINGERPRINT_EPISLON = 12;
        private const int DEFAULT_CYCLES = 3;

        /// <summary>
        /// Detect images that are duplicates and group them together
        /// </summary>
        /// <param name="snapshots">All of the snapshot contexts</param>
        /// <returns>An IEnumerable of IEnumerables that represent groups of similar images</returns>
        public static IEnumerable<IEnumerable<SnapshotContext>> GroupPotientialDuplicates(
            IEnumerable<SnapshotContext> snapshots
        )
        {
            return GroupSortedSnapshots(snapshots.OrderBy(s => s.FingerPrint));
        }

        private static IEnumerable<IEnumerable<SnapshotContext>> GroupSortedSnapshots(IEnumerable<SnapshotContext> snapshots)
        {
            var listOfGroups = new List<IEnumerable<SnapshotContext>>();
            SnapshotContext lastSnapshot = null;
            HashSet<SnapshotContext> lastGroup = null;
            foreach (var snapshot in snapshots)
            {
                if (
                    lastSnapshot == null ||
                    lastGroup == null ||
                    Math.Abs(snapshot.FingerPrint - lastSnapshot.FingerPrint) >= FINGERPRINT_EPISLON
                )
                {
                    lastGroup = new HashSet<SnapshotContext>();
                    listOfGroups.Add(lastGroup);
                }

                lastGroup.Add(snapshot);
                lastSnapshot = snapshot;
            }

            return listOfGroups;
        }

    }
}
