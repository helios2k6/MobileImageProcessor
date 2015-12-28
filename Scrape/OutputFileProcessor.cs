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
 
 /// <summary>
 /// Processes the output file of tesseract
 /// </summary>
 internal static class OutputFileProcessor
 {
     /// <summary>
     /// Tries to get the timestamp from the contents of the output file
     /// from tesseract
     /// </summary>
     public static Maybe<string> TryGetTime(string[] outputFileContents)
     {
         return Maybe<string>.Nothing;
     }
     
     private static Maybe<string> TryGetPrimaryLine(string[] outputFileContents)
     {
         return Maybe<string>.Nothing;
     }

     private static Maybe<string> TryGetPrimarySubstring(string primaryLine)
     {
         return Maybe<string>.Nothing;
     }

     private static Maybe<string> TryDeduceTimeCodeFromStandardFormat(string primarySubstring)
     {
         return Maybe<string>.Nothing;
     }
     
     private static Maybe<string> TryDeduceTimeCodeFromKnownDigitIssue(string primarySubstring)
     {
         return Maybe<string>.Nothing;
     }
 }