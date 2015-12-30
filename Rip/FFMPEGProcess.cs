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
using System.Diagnostics;

namespace Rip
{
    internal sealed class FFMPEGProcess : IDisposable
    {
        private static readonly string FFMPEG_PROC_NAME = "ffmpeg";
        private static readonly int FRAME_WINDOW_OFFSET = 5;

        private readonly Process _process;
        private readonly int _targetFrameNumber;
        private readonly string _targetMediaFile;
        
        private bool _isDisposed;
        private bool _hasExecuted;
        
        public FFMPEGProcess(int targetFrameNumber, string targetMediaFile)
        {
            _process = new Process();
            _targetFrameNumber = targetFrameNumber;
            _targetMediaFile = targetMediaFile;
            _isDisposed = false;
            _hasExecuted = false;
        }
        
        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }
            
            _isDisposed = true;
            _process.Dispose();
        }
        
        public IList<string> Execute()
        {
            if (_hasExecuted)
            {
                throw new InvalidOperationException("This process has already executed");
            }
            _hasExecuted = true;
            _process.StartInfo.UseShellExecute = true;
            _process.StartInfo.FileName = 
            return new List<string>();
        }
    }
}