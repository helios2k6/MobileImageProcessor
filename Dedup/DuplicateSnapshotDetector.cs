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

using Functional.Maybe;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Dedup
{
    /// <summary>
    /// Class for detecting duplicate images or images that are similar enough
    /// </summary>
    internal static class DuplicateSnapshotDetector
    {
        private const int PIXEL_COLOR_EPSILON = 4;

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
                    if (AreSnapshotsSimilarEnough(uncheckedSnapshot, representativeSnapshotContext))
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

        private static bool AreSnapshotsSimilarEnough(SnapshotContext a, SnapshotContext b)
        {
            return (from aSnapshot in a.ScaledDownSnapshot
                    from bSnapshot in b.ScaledDownSnapshot
                    select AreImagesSimilarEnough(aSnapshot, bSnapshot)).OrElse(false);
        }

        private static bool AreImagesSimilarEnough(Image a, Image b)
        {
            // Easy checks to prevent expensive cycling
            if (a.Height != b.Height || a.Width != b.Width)
            {
                return false;
            }

            using (var aSnapshotBitmap = new Bitmap(a))
            {
                using (var bSnapshotBitmap = new Bitmap(b))
                {
                    var nonSimilarPixels = 0;
                    var maxNonSimilarPixels = (a.Height * a.Width) / 10; // 10% pixel non-similarity tolerance
                    foreach (var coordinate in GetPixelCoordinates(a.Width, a.Height))
                    {
                        var x = coordinate.Item1;
                        var y = coordinate.Item2;

                        var aPixel = aSnapshotBitmap.GetPixel(x, y);
                        var bPixel = bSnapshotBitmap.GetPixel(x, y);

                        if (ArePixelsCloseEnough(aPixel, bPixel) == false)
                        {
                            nonSimilarPixels++;
                            if (nonSimilarPixels >= maxNonSimilarPixels)
                            {
                                return false;
                            }
                        }
                    }
                }
            }

            return true;
        }

        private static IEnumerable<Tuple<int, int>> GetPixelCoordinates(int width, int height)
        {
            var knownCoordinates = new HashSet<Tuple<int, int>>();
            var random = new Random();
            var numberOfPixelsToCheck = (int)Math.Floor((width * height * 2.0) / 5.0); // Check 40% of the pixels
            for (int i = 0; i < numberOfPixelsToCheck; i++)
            {
                yield return GetNextTuple(width, height, random, knownCoordinates);
            }
        }

        private static Tuple<int, int> GetNextTuple(
            int width,
            int height,
            Random random,
            HashSet<Tuple<int, int>> knownCoordinates
        )
        {
            while (true)
            {
                var xCoord = random.Next(width);
                var yCoord = random.Next(height);
                var tupleCoord = Tuple.Create(xCoord, yCoord);
                if (knownCoordinates.Contains(tupleCoord))
                {
                    continue;
                }

                knownCoordinates.Add(tupleCoord);
                return tupleCoord;
            }
        }

        private static bool ArePixelsCloseEnough(Color a, Color b)
        {
            return Math.Abs(a.R - b.R) < PIXEL_COLOR_EPSILON &&
                Math.Abs(a.G - b.G) < PIXEL_COLOR_EPSILON &&
                Math.Abs(a.B - b.B) < PIXEL_COLOR_EPSILON;
        }
    }
}
