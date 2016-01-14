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

using System.Collections.Generic;
using System.Diagnostics;

namespace Dispatch
{
    /// <summary>
    /// Represents a generic process that doesn't have any arguments
    /// </summary>
    internal sealed class GeneralProcess : AutoPipingProcess
    {
        /// <summary>
        /// Creates a General Process with the specified process names and arguments
        /// </summary>
        /// <param name="processNames"></param>
        /// <param name="arguments"></param>
        public GeneralProcess(ICollection<string> processNames, string arguments)
            : base(CreateProcess(processNames, arguments))
        {
        }

        /// <summary>
        /// Creates a General Process with the specified process names and no program arguments
        /// </summary>
        /// <param name="processNames"></param>
        public GeneralProcess(ICollection<string> processNames)
            : this(processNames, null)
        {
        }

        private static Process CreateProcess(ICollection<string> processNames, string arguments)
        {
            var process = new Process();
            process.StartInfo.Arguments = arguments;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.FileName = GetProcessName(processNames);
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;

            return process;
        }
    }
}
