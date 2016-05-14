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

using Functional.Maybe;
using Newtonsoft.Json;
using System;
using System.Drawing;
using System.IO;

namespace CommonImageModel
{
    /// <summary>
    /// Common functions that are used across projects
    /// </summary>
    public static class CommonFunctions
    {
        /// <summary>
        /// Read the ImageJobs object from stdin
        /// </summary>
        /// <returns>An ImageJobs object if deserialization was successful</returns>
        public static Maybe<ImageJobs> TryReadStandardIn()
        {
            var stdin = new StreamReader(Console.OpenStandardInput());
            var input = stdin.ReadToEnd();
            try
            {
                return JsonConvert.DeserializeObject<ImageJobs>(input).ToMaybe();
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Could not deserialize input. {0}", e.Message);
            }

            return Maybe<ImageJobs>.Nothing;
        }

        /// <summary>
        /// Closes all standard file handles to signal the EOF to downstream processes
        /// </summary>
        public static void CloseAllStandardFileHandles()
        {
            // Close everything afterwards
            Console.In.Close();
            Console.Out.Close();
            Console.Error.Close();
        }

        /// <summary>
        /// Attempt to load an Image from the provided image path
        /// </summary>
        /// <param name="imagePath"></param>
        /// <returns></returns>
        public static Maybe<Image> TryLoadImage(string imagePath)
        {
            try
            {
                return Image.FromFile(imagePath).ToMaybe();
            }
            catch (OutOfMemoryException e)
            {
                Console.Error.WriteLine("Could not read image {0}. Reason: {1}", imagePath, e.Message);
            }
            catch (FileNotFoundException e)
            {
                Console.Error.WriteLine("Could not find file {0}. {1}", imagePath, e.Message);
            }
            catch (ArgumentException)
            {
                Console.Error.WriteLine("URIs are not supported. {0}", imagePath);
            }

            return Maybe<Image>.Nothing;
        }

        /// <summary>
        /// Load the Image as a LockBitImage instead of as a regular image
        /// </summary>
        /// <param name="imagePath">The path to the image file</param>
        /// <returns>A LockBitImage if this was successful or None if not</returns>
        public static Maybe<LockBitImage> TryLoadImageAsLockBit(string imagePath)
        {
            return from image in TryLoadImage(imagePath)
                   select ExecThenDispose<LockBitImage>(
                        () => new LockBitImage(image),
                        image
                   );
        }

        /// <summary>
        /// Execute some function that returns a T and then dispose of any resources
        /// </summary>
        /// <typeparam name="T">The type T</typeparam>
        /// <param name="func">The function to execute</param>
        /// <param name="disposables">The array of disposables</param>
        /// <returns>Whatever Func was supposed to return</returns>
        public static T ExecThenDispose<T>(
            Func<T> func,
            params IDisposable[] disposables
        )
        {
            T result = func.Invoke();
            foreach (var disposable in disposables)
            {
                disposable.Dispose();
            }

            return result;
        }
    }
}
