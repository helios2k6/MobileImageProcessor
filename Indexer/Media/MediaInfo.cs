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
using System.Linq;
using YAXLib;

namespace Indexer.Media
{
    [Serializable]
    [YAXSerializeAs("Mediainfo")]
    internal sealed class MediaInfo : IEquatable<MediaInfo>
    {
        #region private fields
        private static readonly string GENERAL_TRACK = "General";

        private readonly Lazy<Track> _generalTrack;
        private readonly Lazy<TimeSpan> _duration;
        #endregion

        #region ctor
        public MediaInfo()
        {
            _generalTrack = new Lazy<Track>(CalculateGeneralTrack);
            _duration = new Lazy<TimeSpan>(CalculateDuration);
        }
        #endregion

        #region public properties
        [YAXSerializeAs("File")]
        public FileXMLNode File { get; set; }
        #endregion

        #region public methods
        /// <summary>
        /// Get the name of the file that this MediaInfo describes
        /// </summary>
        /// <returns>The file name or string.Empty if it cannot be determined</returns>
        public string GetFileName()
        {
            return _generalTrack.Value?.CompleteName ?? string.Empty;
        }

        /// <summary>
        /// Get the duration of this media file, if possible
        /// </summary>
        /// <returns>
        /// Returns the duration of this media file as a TimeSpan or a TimeSpan of 0 if the duration
        /// cannot be determined
        /// </returns>
        public TimeSpan GetDuration()
        {
            return _duration.Value;
        }
        
        /// <summary>
        /// Get the framerate of the video track
        /// </summary>
        public FPS GetFramerate()
        {
            throw new NotImplementedException();
        }
        
        public bool Equals(MediaInfo other)
        {
            if (EqualsPreamble(other) == false)
            {
                return false;
            }

            return Equals(File, other.File);
        }

        public override bool Equals(object other)
        {
            return Equals(other as MediaInfo);
        }

        public override int GetHashCode()
        {
            return File.GetHashCode();
        }
        #endregion

        #region private methods
        private TimeSpan CalculateDuration()
        {
            return _generalTrack.Value?.GetDurationAsTimeSpan() ?? TimeSpan.FromSeconds(0);
        }
        
        private Track CalculateGeneralTrack()
        {
            return (from track in File.Tracks
                    where string.Equals(track.Type, GENERAL_TRACK, StringComparison.OrdinalIgnoreCase)
                    select track).SingleOrDefault();
        }

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