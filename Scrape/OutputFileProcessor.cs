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
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Processes the output file of tesseract
/// </summary>
internal static class OutputFileProcessor
{
    /// <summary>
    /// Tries to get the timestamp from the contents of the output file
    /// from tesseract
    /// </summary>
    public static Maybe<TimeSpan> TryGetTime(string[] outputFileContents)
    {
        return TryGetPrimaryLine(outputFileContents)
           .Select(
               line => TryDeduceTimeCodeFromStandardFormat(line)
                           .Or(TryDeduceTimeCodeFromKnownDigitIssue(line))
           );
    }

    private static Maybe<string> TryGetPrimaryLine(string[] outputFileContents)
    {
        foreach (var line in outputFileContents)
        {
            var indexOfDone = line.IndexOf("DONE", StringComparison.OrdinalIgnoreCase);
            if (indexOfDone != -1)
            {
                return line.Substring(indexOfDone + 4).Trim().ToMaybe();
            }
        }
        return Maybe<string>.Nothing;
    }

    private static Maybe<TimeSpan> TryDeduceTimeCodeFromStandardFormat(string primarySubstring)
    {
        var explodedStrings = primarySubstring.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries).Reverse().ToList();
        if (explodedStrings.Count < 2)
        {
            return Maybe<TimeSpan>.Nothing;
        }

        var secondsString = explodedStrings[0];
        var minutesString = explodedStrings[1];

        int seconds, minutes, hours;
        seconds = minutes = hours = 0;

        if (int.TryParse(secondsString, out seconds) && int.TryParse(minutesString, out minutes))
        {
            if (explodedStrings.Count > 2)
            {
                var hoursString = explodedStrings[2];
                if (int.TryParse(hoursString, out hours) == false)
                {
                    // Ensure proper assignment
                    hours = 0;
                }
            }

            return new TimeSpan(hours, minutes, seconds).ToMaybe();
        }

        return Maybe<TimeSpan>.Nothing;
    }

    private static Maybe<TimeSpan> TryDeduceTimeCodeFromKnownDigitIssue(string primarySubstring)
    {
        var reversedPrimarySubstring = primarySubstring.Reverse().ToArray();
        int currentIndex = 0;
        var secondsChars = new List<char>();
        var minutesChars = new List<char>();
        var hoursChars = new List<char>();
        foreach (var currentChar in reversedPrimarySubstring)
        {
            if (currentIndex < 2)
            {
                secondsChars.Add(currentChar);
            }
            else if (currentIndex < 4)
            {
                minutesChars.Add(currentChar);
            }
            else if (currentIndex > 4)
            {
                hoursChars.Add(currentChar);
            }
            currentIndex++;
        }
        
        secondsChars.Reverse();
        minutesChars.Reverse();
        hoursChars.Reverse();
        
        var secondsString = new string(secondsChars.ToArray());
        var minutesString = new string(minutesChars.ToArray());
        var hoursString = new string(hoursChars.ToArray());
        
        int seconds, minutes, hours;
        seconds = minutes = hours = 0;
        
        if (int.TryParse(secondsString, out seconds) && int.TryParse(minutesString, out minutes))
        {
            if (int.TryParse(hoursString, out hours) == false)
            {
                hours = 0;
            }
            
            return new TimeSpan(hours, minutes, seconds).ToMaybe();
        }
        
        return Maybe<TimeSpan>.Nothing;
    }
}