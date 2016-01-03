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
using System.Drawing;
using System.Linq;

namespace Match
{
    /// <summary>
    /// Matches images together to detect whether they are the same
    /// </summary>
    internal static class ImageMatcher
    {
        /// <summary>
        /// Detects the best matching snapshot for an ImageJob
        /// </summary>
        /// <param name="imageJobs">The ImageJobs to match</param>
        /// <returns>A dictionary between an ImageJob its matching snapshot</returns>
        public static IDictionary<ImageJob, string> GetMatches(ImageJobs imageJobs)
        {
            var resultMap = new Dictionary<ImageJob, string>();
            foreach (var imageJob in imageJobs.Images)
            {
                TryFindMatchingSnapshot(imageJob).Apply(candidateSnapshot => resultMap.Add(imageJob, candidateSnapshot));
            }

            return resultMap;
        }

        private static Maybe<string> TryFindMatchingSnapshot(ImageJob imageJob)
        {
            var candidateSnapshots = imageJob.ImageSnapshots;
            return from originalImage in TryLoadImage(imageJob.OriginalFilePath)
                   select ExecThenDispose(
                    () => TryProcessSnapshotFromiPad(originalImage, candidateSnapshots)
                            .Or(TryProcessSnapshotFromiPhoneSix(originalImage, candidateSnapshots)),
                    originalImage
                   );
        }

        private static Maybe<string> TryProcessSnapshotFromiPad(
            ImageWrapper originalSnapshot,
            IEnumerable<string> candidateSnapshots
        )
        {
            if (Predicates.IsiPadSize(originalSnapshot.Image) == false)
            {
                return Maybe<string>.Nothing;
            }

            return from croppedImage in ImageCropper.TryCropiPadImage(originalSnapshot)
                   from resizedImage in ImageTransformations.TryResizeImage(croppedImage.Image, 1280, 720)
                   let transformedOriginalSnapshot = new ImageWrapper(resizedImage, originalSnapshot.ImagePath)
                   select ExecThenDispose(
                        () => TryDeduceMatchingCandidateSnapshot(transformedOriginalSnapshot, candidateSnapshots, i => i.ToMaybe()),
                        croppedImage,
                        resizedImage
                   );
        }

        private static Maybe<string> TryProcessSnapshotFromiPhoneSix(
            ImageWrapper originalSnapshot,
            IEnumerable<string> candidateSnapshots
        )
        {
            if (Predicates.IsiPhoneSixSize(originalSnapshot.Image) == false)
            {
                return Maybe<string>.Nothing;
            }

            return from croppedOriginalSnapshot in ImageCropper.TryCropiPhoneSixImage(originalSnapshot)
                   select ExecThenDispose(
                            () => TryDeduceMatchingCandidateSnapshot(
                                croppedOriginalSnapshot,
                                candidateSnapshots,
                                TransformCandidateSnapshotForiPhoneSix
                            ),
                            croppedOriginalSnapshot
                   );
        }

        private static Maybe<ImageWrapper> TransformCandidateSnapshotForiPhoneSix(ImageWrapper candidateSnapshot)
        {
            return from resizedCandidateSnapshot in ImageTransformations.TryResizeImage(candidateSnapshot.Image, 1334, 750)
                   select ImageCropper.TryCropiPhoneSixImage(
                        new ImageWrapper(resizedCandidateSnapshot, candidateSnapshot.ImagePath)
                   );
        }

        private static Maybe<string> TryDeduceMatchingCandidateSnapshot(
            ImageWrapper transformedOriginalSnapshot,
            IEnumerable<string> candidateSnapshots,
            Func<ImageWrapper, Maybe<ImageWrapper>> candidateTranform
        )
        {
            var candidateResultList = new List<Tuple<string, int>>();
            foreach (var candidateSnapshot in candidateSnapshots)
            {
                var candidateComparisonResult = from loadedCandidateSnapshot in TryLoadImage(candidateSnapshot)
                                                from transformedCandidate in candidateTranform.Invoke(loadedCandidateSnapshot)
                                                let similarityIndex = SimilarityCalculator.CalculateSimilarityIndex(
                                                    transformedOriginalSnapshot,
                                                    transformedCandidate
                                                )
                                                select ExecThenDispose(
                                                    () => Tuple.Create<string, int>(candidateSnapshot, similarityIndex),
                                                    loadedCandidateSnapshot,
                                                    transformedCandidate
                                                );

                candidateComparisonResult.Apply(t => candidateResultList.Add(t));
            }

            candidateResultList.Sort((a, b) => b.Item2 - a.Item2);
            return candidateResultList.FirstMaybe().Select(t => t.Item1);
        }

        private static Maybe<ImageWrapper> TryLoadImage(string path)
        {
            return from image in CommonFunctions.TryLoadImage(path)
                   select new ImageWrapper(image, path);
        }

        private static T ExecThenDispose<T>(
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
