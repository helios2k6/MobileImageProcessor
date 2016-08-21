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
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Indexer
{
    /// <summary>
    /// Represents an entry in the indexing file
    /// </summary>
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    internal sealed class IndexEntry : IEquatable<IndexEntry>, ISerializable
    {
        #region public properties
        /// <summary>
        /// The video file that was indexed
        /// </summary>
        [JsonProperty(PropertyName = "VideoFile", Required = Required.Always)]
        public string VideoFile { get; set; }

        /// <summary>
        /// The start time of the time-window that this frame occurs in
        /// </summary>
        [JsonProperty(PropertyName = "StartTime", Required = Required.Always)]
        public TimeSpan StartTime { get; set; }

        /// <summary>
        /// The end time of the time-window that this frame occurs in
        /// </summary>
        [JsonProperty(PropertyName = "EndTime", Required = Required.Always)]
        public TimeSpan EndTime { get; set; }

        /// <summary>
        /// The frame's hash (fingerprint)
        /// </summary>
        [JsonProperty(PropertyName = "FrameHash", Required = Required.Always)]
        public ImageFingerPrint FrameHash { get; set; }
        #endregion

        #region ctor
        public IndexEntry()
        {
            VideoFile = string.Empty;
            FrameHash = new ImageFingerPrint();
        }

        public IndexEntry(SerializationInfo info, StreamingContext context)
        {
            VideoFile = info.GetString("VideoFile");
            StartTime = (TimeSpan)info.GetValue("StartTime", typeof(TimeSpan));
            EndTime = (TimeSpan)info.GetValue("EndTime", typeof(TimeSpan));
            FrameHash = (ImageFingerPrint)info.GetValue("FrameHash", typeof(ImageFingerPrint));
        }
        #endregion

        #region public methods
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("VideoFile", VideoFile);
            info.AddValue("StartTime", StartTime);
            info.AddValue("EndTime", EndTime);
            info.AddValue("FrameHash", FrameHash);
        }

        public override string ToString()
        {
            return string.Format(
                "{0} between [{1}, {2}] with fingerprint {3}",
                VideoFile,
                StartTime,
                EndTime,
                FrameHash
            );
        }

        public override bool Equals(object other)
        {
            return Equals(other as IndexEntry);
        }

        public override int GetHashCode()
        {
            return VideoFile.GetHashCode() ^
                StartTime.GetHashCode() ^
                FrameHash.GetHashCode();
        }

        public bool Equals(IndexEntry other)
        {
            if (EqualsPreamble(other) == false)
            {
                return false;
            }

            return string.Equals(VideoFile, other.VideoFile, StringComparison.Ordinal) &&
                Equals(StartTime, other.StartTime) &&
                Equals(EndTime, other.EndTime) &&
                Equals(FrameHash, other.FrameHash);
        }
        #endregion

        #region private methods
        private bool EqualsPreamble(object other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            if (GetType() != other.GetType()) return false;

            return true;
        }
        #endregion
    }
}
