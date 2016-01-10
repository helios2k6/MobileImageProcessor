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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Dispatch
{
    /// <summary>
    /// Represents the Process framework 
    /// </summary>
    internal abstract class AutoPipingProcess : IDisposable
    {
        private readonly Process _process;
        private bool _didStartOnce;

        protected AutoPipingProcess(Process process)
        {
            _process = process;
            _didStartOnce = false;
        }

        protected static string GetProcessName(ICollection<string> processNames)
        {
            foreach (var processName in processNames)
            {
                if (File.Exists(processName))
                {
                    return processName;
                }
            }

            throw new InvalidOperationException("Could not find the Slice executable");
        }

        public Task<string> ExecuteAsync(Maybe<string> pipeIn)
        {
            if (_didStartOnce)
            {
                throw new InvalidOperationException("Process already executed");
            }

            _didStartOnce = true;

            return Task.Factory.StartNew<string>(() => ExecuteInternal(pipeIn));
        }

        public void Dispose()
        {
            _process.Dispose();
        }

        private string ExecuteInternal(Maybe<string> pipeIn)
        {
            StartProcess();
            pipeIn.Apply(WriteToStandardIn);
            var output = GetOutputAndWait();
            CheckExitCode();

            return output;
        }

        private void CheckExitCode()
        {
            if (_process.ExitCode != 0)
            {
                throw new InvalidOperationException("Process did not execute properly");
            }
        }

        private string GetOutputAndWait()
        {
            var output = _process.StandardOutput.ReadToEnd();
            _process.WaitForExit();
            return output;
        }

        private void WriteToStandardIn(string input)
        {
            _process.StandardInput.Write(input);
        }

        private void StartProcess()
        {
            bool didStart = _process.Start();
            if (didStart == false)
            {
                throw new InvalidOperationException("Process did not start properly");
            }
        }
    }
}
