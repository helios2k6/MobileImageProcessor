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
using Indexer.Media;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Indexer
{
    internal static class Indexer
    {
        #region public methods
        public async static Task<IEnumerable<IndexEntry>> IndexVideoAsync(string videoFile)
        {
            MediaInfo info = await GetMediaInfoAsync(videoFile);
            return await GetIndexEntriesAsync(info);
        }
        #endregion

        #region private methods
        private static Task<MediaInfo> GetMediaInfoAsync(string videoFile)
        {
            return Task.Factory.StartNew(() =>
            {
                return (new MediaInfoProcess(videoFile)).Execute();
            });
        }

        private static Task<IEnumerable<IndexEntry>> GetIndexEntriesAsync(MediaInfo info)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}