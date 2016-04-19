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
using System.Drawing;
using System.Linq;

namespace CommonImageModel
{
    /// <summary>
    /// Class that fingerprints an image
    /// </summary>
    public static class ImageFingerPrinter
    {
        private const int MACROBLOCK_LENGTH = 4;
        
        /// <summary>
        /// Attempts to calculate the fingerprint of the image by automatically loading the 
        /// image specified by pathToImageFile and calculating its fingerprint
        /// </summary>
        /// <param name="pathToImageFile">The path to the image file</param>
        /// <returns>A image finger print or None if the image could not be loaded</returns>
        public static Maybe<ImageFingerPrint> TryCalculateFingerPrint(string pathToImageFile)
        {
            return from lockBitImage in CommonFunctions.TryLoadImageAsLockBit(pathToImageFile)
                   select CommonFunctions.ExecThenDispose(
                       () => CalculateFingerPrint(lockBitImage),
                       lockBitImage
                   );
        }
        
        /// <summary>
        /// Calculates the FingerPrint of the LockBitImage
        /// </summary>
        /// <param name="image">The LockBitImage</param>
        /// <returns>A FingerPrint representing this LockBitImage</returns>
        public static ImageFingerPrint CalculateFingerPrint(LockBitImage image)
        {
            var cropWindow = new Size(MACROBLOCK_LENGTH, MACROBLOCK_LENGTH);

            var topLeftPoint = new Point(0, 0);
            var topRightPoint = new Point(image.Width - MACROBLOCK_LENGTH, 0);
            var centerPoint = new Point((image.Width / 2) - (MACROBLOCK_LENGTH / 2), (image.Height / 2) - (MACROBLOCK_LENGTH / 2));
            var bottomLeftPoint = new Point(0, image.Height - MACROBLOCK_LENGTH);
            var bottomRightPoint = new Point(image.Width - MACROBLOCK_LENGTH, image.Height - MACROBLOCK_LENGTH);

            var focusTopLeftPoint = new Point((image.Width / 3) - (MACROBLOCK_LENGTH / 2), (image.Height / 3) - (MACROBLOCK_LENGTH / 2));
            var focusTopRightPoint = new Point((image.Width * 2 / 3) - (MACROBLOCK_LENGTH / 2), (image.Height / 3) - (MACROBLOCK_LENGTH / 2));
            var focusBottomLeftPoint = new Point((image.Width / 3) - (MACROBLOCK_LENGTH / 2), (image.Height * 2 / 3) - (MACROBLOCK_LENGTH / 2));
            var focusBottomRightPoint = new Point((image.Width * 2 / 3) - (MACROBLOCK_LENGTH / 2), (image.Height * 2 / 3) - (MACROBLOCK_LENGTH / 2));

            return new ImageFingerPrint(
                GetMacroblock(image, new Rectangle(topLeftPoint, cropWindow)),
                GetMacroblock(image, new Rectangle(topRightPoint, cropWindow)),
                GetMacroblock(image, new Rectangle(centerPoint, cropWindow)),
                GetMacroblock(image, new Rectangle(bottomLeftPoint, cropWindow)),
                GetMacroblock(image, new Rectangle(bottomRightPoint, cropWindow)),
                GetMacroblock(image, new Rectangle(focusTopLeftPoint, cropWindow)),
                GetMacroblock(image, new Rectangle(focusTopRightPoint, cropWindow)),
                GetMacroblock(image, new Rectangle(focusBottomLeftPoint, cropWindow)),
                GetMacroblock(image, new Rectangle(focusBottomRightPoint, cropWindow))
            );
        }

        private static Macroblock GetMacroblock(LockBitImage image, Rectangle cropArea)
        {
            Color[,] colorGrid = new Color[cropArea.Width, cropArea.Height];
            int colorGridY = 0;
            foreach (var y in Enumerable.Range(cropArea.Y, cropArea.Height))
            {
                int colorGridX = 0;
                foreach (var x in Enumerable.Range(cropArea.X, cropArea.Width))
                {
                    colorGrid[colorGridY, colorGridX] = image.GetPixel(x, y);
                    colorGridX++;
                }

                colorGridY++;
            }

            return new Macroblock(colorGrid);
        }
    }
}
