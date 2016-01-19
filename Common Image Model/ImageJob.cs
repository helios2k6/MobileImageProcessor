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

using Newtonsoft.Json;
using System;
using System.Linq;

namespace CommonImageModel
{
    /// <summary>
    /// Represents all of the context required to process an image and find its original
    /// frame
    /// </summary>
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class ImageJob : IEquatable<ImageJob>
    {
        /// <summary>
        /// The path to the original file
        /// </summary>
        [JsonProperty(PropertyName = "OriginalFilePath", Required = Required.Always)]
        public string OriginalFilePath { get; set; }

        /// <summary>
        /// The path to the image that contains the image to the time
        /// information for the original image
        /// </summary>
        [JsonProperty(PropertyName = "SliceImagePath", Required = Required.AllowNull)]
        public string SliceImagePath { get; set; }

        /// <summary>
        /// The timestamp at which the screenshot was taken
        /// </summary>
        [JsonProperty(PropertyName = "SnapshotTimeStamp", Required = Required.AllowNull)]
        public TimeSpan SnapshotTimestamp { get; set; }

        /// <summary>
        /// The images that were taken from the original media file
        /// </summary>
        [JsonProperty(PropertyName = "ImageSnapshots", Required = Required.AllowNull)]
        public string[] ImageSnapshots { get; set; }

        public bool Equals(ImageJob other)
        {
            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return string.Equals(OriginalFilePath, other.OriginalFilePath, StringComparison.Ordinal) &&
                string.Equals(SliceImagePath, other.SliceImagePath, StringComparison.Ordinal) &&
                Equals(SnapshotTimestamp, other.SnapshotTimestamp) &&
                Enumerable.SequenceEqual(ImageSnapshots, other.ImageSnapshots);
        }

        // override object.Equals
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return Equals(obj as ImageJob);
        }

        public override int GetHashCode()
        {
            return OriginalFilePath.GetHashCode() ^
                GetHashCodeOrZero(SliceImagePath) ^
                SnapshotTimestamp.GetHashCode() ^
                GetHashCodeOrZero(ImageSnapshots);
        }

        public override string ToString()
        {
            return OriginalFilePath;
        }

        private static int GetHashCodeOrZero(object obj)
        {
            if (obj == null)
            {
                return 0;
            }

            return obj.GetHashCode();
        }
    }
}
