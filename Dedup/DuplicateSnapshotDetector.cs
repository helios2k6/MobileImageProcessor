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

using System.Collections.Generic;
using System.Linq;

namespace Dedup
{
    /// <summary>
    /// Class for detecting duplicate images or images that are similar enough
    /// </summary>
    internal static class DuplicateSnapshotDetector
    {
        /// <summary>
        /// Detect images that are duplicates and group them together
        /// </summary>
        /// <param name="snapshots">All of the snapshot contexts</param>
        /// <returns>An IEnumerable of IEnumerables that represent groups of similar images</returns>
        public static IEnumerable<IEnumerable<SnapshotContext>> DetectDuplicates(IEnumerable<SnapshotContext> snapshots)
        {
            // Ensure we have at least one element
            if (snapshots.Any() == false)
            {
                return Enumerable.Empty<IEnumerable<SnapshotContext>>();
            }

            var uncheckedSnapshots = new List<SnapshotContext>(snapshots);
            var groupsOfSnapshots = new HashSet<HashSet<SnapshotContext>>();

            foreach (SnapshotContext uncheckedSnapshot in uncheckedSnapshots)
            {
                // Cycle through all groups to see if it belongs in any
                var addedToGroup = false;
                foreach (HashSet<SnapshotContext> group in groupsOfSnapshots)
                {
                    SnapshotContext representativeSnapshotContext = group.First();
                    if (uncheckedSnapshot.FingerPrint.IsSimilarTo(representativeSnapshotContext.FingerPrint))
                    {
                        group.Add(uncheckedSnapshot);
                        addedToGroup = true;
                        break;
                    }
                }

                // Otherwise, create its own group 
                if (addedToGroup == false)
                {
                    var snapshotGroup = new HashSet<SnapshotContext>();
                    snapshotGroup.Add(uncheckedSnapshot);
                    groupsOfSnapshots.Add(snapshotGroup);
                }
            }

            return groupsOfSnapshots;
        }
    }
}
