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
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Rip
{
    /// <summary>
    /// Harvests all possible images for a given ImageJob
    /// </summary>
    internal static class SnapshotHarvester
    {
        /// <summary>
        /// Harvest all of the possible images for a given ImageJob
        /// </summary>
        public static IEnumerable<string> HarvestCandidateImages(ImageJob imageJob, IEnumerable<string> folderPaths)
        {
            return Enumerable.Empty<string>();
        }
        
        private static IEnumerable<string> GetAllMediaFiles(IEnumerable<string> folderPaths)
        {
            return folderPaths.SelectMany(GetAllMediaFiles);
        }
        
        private static IEnumerable<string> GetAllMediaFiles(string folderPath)
        {
            var listOfFileLists = new[] 
            {
                Directory.EnumerateFiles(folderPath, "*.mkv", SearchOption.AllDirectories),
                Directory.EnumerateFiles(folderPath, "*.mp4", SearchOption.AllDirectories),
                Directory.EnumerateFiles(folderPath, "*.wmv", SearchOption.AllDirectories),
            };
            return listOfFileLists.SelectMany(x => x);
        }
        
        private static IEnumerable<string> GetImages(ImageJob imageJob, IEnumerable<string> mediaFiles)
        {
            
            return Enumerable.Empty<string>();
        }
    }
}