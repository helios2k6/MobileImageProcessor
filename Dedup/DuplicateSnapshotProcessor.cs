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
using System.IO;
using System.Linq;

namespace Dedup
{
    /// <summary>
    /// Processes and deletes duplicate images
    /// </summary>
    internal static class DuplicateSnapshotProcessor
    {
        /// <summary>
        /// Deletes duplicate snapshots and returns a list of snapshot contexts that were not deleted
        /// </summary>
        /// <param name="duplicateGroups">The groups of duplicate images</param>
        /// <returns>An IEnumerable of snapshots that were not deleted</returns>
        public static IEnumerable<SnapshotContext> DeleteDuplicateImages(
            IEnumerable<IEnumerable<SnapshotContext>> duplicateGroups
        )
        {
            return duplicateGroups.Select(DeleteDuplicates);
        }

        private static SnapshotContext DeleteDuplicates(IEnumerable<SnapshotContext> group)
        {
            var firstSnapshot = group.First();
            foreach (var snapshot in group.Where(s => ReferenceEquals(firstSnapshot, s) == false))
            {
                File.Delete(snapshot.SnapshotPath);
            }
            return firstSnapshot;
        }
    }
}
