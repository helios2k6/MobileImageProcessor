﻿/* 
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
using Newtonsoft.Json;
using System;

namespace Indexer
{
    /// <summary>
    /// Represents an entry in the indexing file
    /// </summary>
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    internal sealed class IndexEntry : IEquatable<IndexEntry>
    {
        /// <summary>
        /// The video file that was indexed
        /// </summary>
        [JsonProperty(PropertyName = "VideoFile", Required = Required.Always)]
        public string VideoFile { get; set; }

        /// <summary>
        /// The timestamp of the frame
        /// </summary>
        [JsonProperty(PropertyName = "FrameTimeStamp", Required = Required.Always)]
        public DateTime FrameTimeStamp { get; set; }

        /// <summary>
        /// The frame's hash (fingerprint)
        /// </summary>
        [JsonProperty(PropertyName = "FrameHash", Required = Required.Always)]
        public ImageFingerPrint FrameHash { get; set; }

        public override string ToString()
        {
            return string.Format(
                "{0} at {1} with fingerprint {2}",
                VideoFile,
                FrameTimeStamp,
                FrameHash
            );
        }

        public override bool Equals(object other)
        {
            if (other == null || GetType() != other.GetType())
            {
                return false;
            }

            return Equals(other as IndexEntry);
        }

        public override int GetHashCode()
        {
            return VideoFile.GetHashCode() ^
                FrameTimeStamp.GetHashCode() ^
                FrameHash.GetHashCode();
        }

        public bool Equals(IndexEntry other)
        {
            if (other == null || GetType() != other.GetType())
            {
                return false;
            }

            if (this == other)
            {
                return true;
            }

            return string.Equals(VideoFile, other.VideoFile, StringComparison.Ordinal) &&
                DateTime.Equals(FrameTimeStamp, other.FrameTimeStamp) &&
                Equals(FrameHash, other.FrameHash);
        }
    }
}