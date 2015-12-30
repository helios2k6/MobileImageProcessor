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
using System.Diagnostics;
using System.IO;

namespace Scrape
{
    /// <summary>
    /// Represents the "tesseract" executable that is expected to be
    /// on the PATH. 
    /// </summary>
    /// <remarks>
    /// This class is not asynchronous and is meant to be run in its own thread.
    /// Calling the "Execute()" method on this class will block the current thread
    /// until the child process is finished. 
    /// </remarks>
    public sealed class TesseractProcess : IDisposable
    {
        private static readonly string TESSERACT_PROC_NAME = "tesseract";

        private readonly Process _process;
        private readonly string _imagePath;
        private readonly string _outputArgument;
        private volatile bool _hasExecuted;

        /// <summary>
        /// Construct a new TesseractProcess object, specifying the image
        /// path that will be fed into the "tesseract" executable
        /// </summary>
        public TesseractProcess(string imagePath)
        {
            if (File.Exists(imagePath) == false)
            {
                throw new ArgumentException(
                    "File not found. The following image does not exist: {0}",
                    imagePath
                );
            }
            
            _process = new Process();
            _imagePath = imagePath;

            _outputArgument = string.Format(
                "{0}_OCR_RESULT", 
                Path.GetFileNameWithoutExtension(imagePath)
            );
            _hasExecuted = false;
        }

        public void Dispose()
        {
            _process.Dispose();
        }

        /// <summary>
        /// Execute this process and return all output from the stdout pipe
        /// of the tesseract executable
        /// </summary>
        /// <remarks>
        /// This is a blocking function and will block the thread it is executed on
        /// until the underlying tesseract executable is finished.
        /// </remarks>
        public string[] Execute()
        {
            if (_hasExecuted) 
            {
                throw new InvalidOperationException("Cannot execute the Tesseract Process twice");
            }
            _hasExecuted = true;
            _process.StartInfo.UseShellExecute = true;
            _process.StartInfo.FileName = CalculateProcessName();
            _process.StartInfo.CreateNoWindow = true;
            _process.StartInfo.Arguments = string.Format("\"{0}\" {1}", _imagePath, _outputArgument);

            var processStarted = _process.Start();
            if (processStarted == false)
            {
                throw new Exception("Unable to start the tesseract executable");
            }

            _process.WaitForExit();
            
            if (_process.ExitCode != 0)
            {
                throw new Exception("Tesseract did not execute properly.");
            }
            
            var outputFilePathWithExtension = string.Format("{0}.txt", _outputArgument);
            if (File.Exists(outputFilePathWithExtension) == false)
            {
                throw new FileNotFoundException("Output file not found.");
            }
            
            var outputFileContents = File.ReadAllLines(outputFilePathWithExtension);
            File.Delete(outputFilePathWithExtension);
            return outputFileContents;
        }
        
        private static string CalculateProcessName()
        {
            var operationSystem = Environment.OSVersion;
            switch (operationSystem.Platform)
            {
                case PlatformID.Win32NT:
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                case PlatformID.WinCE:
                    return TESSERACT_PROC_NAME + ".exe";
                case PlatformID.MacOSX:
                case PlatformID.Unix:
                    return TESSERACT_PROC_NAME;
                case PlatformID.Xbox:
                    throw new InvalidOperationException("Xbox OS not supported");
                default:
                    throw new InvalidOperationException("Unknown OS detected");
            }
        }
    }
}

