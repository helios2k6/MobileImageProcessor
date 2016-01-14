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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        public static IDictionary<ImageJob, IEnumerable<string>> GetMatches(ImageJobs imageJobs)
        {
            return imageJobs.Images.ToDictionary(i => i, TryFindMatchingSnapshots);
        }

        private static IEnumerable<string> TryFindMatchingSnapshots(ImageJob imageJob)
        {
            var candidateSnapshots = imageJob.ImageSnapshots;
            var deducedImages = from originalImage in TryLoadImage(imageJob.OriginalFilePath)
                                select CommonFunctions.ExecThenDispose(
                                 () => TryProcessSnapshotFromiPad(originalImage, candidateSnapshots)
                                         .Or(TryProcessSnapshotFromiPhoneSix(originalImage, candidateSnapshots)),
                                 originalImage
                                );

            return deducedImages.OrElse(Enumerable.Empty<string>());
        }

        private static IEnumerable<string> TryProcessSnapshotFromiPad(
            ImageWrapper originalSnapshot,
            IEnumerable<string> candidateSnapshots
        )
        {
            if (Predicates.IsiPadSize(originalSnapshot.Image) == false)
            {
                return Enumerable.Empty<string>();
            }

            var deducedSnapshots = from croppedImage in ImageCropper.TryCropiPadImage(originalSnapshot)
                                   from resizedImage in ImageTransformations.TryResizeImage(croppedImage.Image, 1280, 720)
                                   let transformedOriginalSnapshot = new ImageWrapper(resizedImage, originalSnapshot.ImagePath)
                                   select CommonFunctions.ExecThenDispose(
                                        () => TryDeduceMatchingCandidateSnapshot(transformedOriginalSnapshot, candidateSnapshots, i => i.ToMaybe()),
                                        croppedImage,
                                        resizedImage
                                   );

            return deducedSnapshots.OrElse(Enumerable.Empty<string>());
        }

        private static IEnumerable<string> TryProcessSnapshotFromiPhoneSix(
            ImageWrapper originalSnapshot,
            IEnumerable<string> candidateSnapshots
        )
        {
            if (Predicates.IsiPhoneSixSize(originalSnapshot.Image) == false)
            {
                return Enumerable.Empty<string>();
            }

            Maybe<IEnumerable<string>> deducedSnapshots = from croppedOriginalSnapshot in ImageCropper.TryCropiPhoneSixImage(originalSnapshot)
                                                          select CommonFunctions.ExecThenDispose(
                                                                   () => TryDeduceMatchingCandidateSnapshot(
                                                                       croppedOriginalSnapshot,
                                                                       candidateSnapshots,
                                                                       TransformCandidateSnapshotForiPhoneSix
                                                                   ),
                                                                   croppedOriginalSnapshot
                                                          );

            return deducedSnapshots.OrElse(Enumerable.Empty<string>());
        }

        private static Maybe<ImageWrapper> TransformCandidateSnapshotForiPhoneSix(ImageWrapper candidateSnapshot)
        {
            return from resizedCandidateSnapshot in ImageTransformations.TryResizeImage(candidateSnapshot.Image, 1334, 750)
                   select ImageCropper.TryCropiPhoneSixImage(
                        new ImageWrapper(resizedCandidateSnapshot, candidateSnapshot.ImagePath)
                   );
        }

        private static IEnumerable<string> TryDeduceMatchingCandidateSnapshot(
            ImageWrapper transformedOriginalSnapshot,
            IEnumerable<string> candidateSnapshots,
            Func<ImageWrapper, Maybe<ImageWrapper>> candidateTranform
        )
        {
            var candidateResultList = new ConcurrentBag<Tuple<string, int>>();
            using (var originaImageAsLockbit = new LockBitImage(transformedOriginalSnapshot.Image))
            {
                Parallel.ForEach(candidateSnapshots, candidateSnapshot =>
                {
                    var candidateComparisonResult = from loadedCandidateSnapshot in TryLoadImage(candidateSnapshot)
                                                    from transformedCandidate in candidateTranform.Invoke(loadedCandidateSnapshot)
                                                    let candidateLockbitImage = new LockBitImage(transformedCandidate.Image)
                                                    let similarityIndex = SimilarityCalculator.CalculateSimilarityIndex(
                                                        originaImageAsLockbit,
                                                        candidateLockbitImage
                                                    )
                                                    select CommonFunctions.ExecThenDispose(
                                                        () => Tuple.Create<string, int>(candidateSnapshot, similarityIndex),
                                                        loadedCandidateSnapshot,
                                                        transformedCandidate,
                                                        candidateLockbitImage
                                                    );
                    candidateComparisonResult.Apply(i => candidateResultList.Add(i));
                });
            }

            return from candidateTuple in candidateResultList
                   where candidateTuple.Item2 >= 69
                   select candidateTuple.Item1;
        }

        private static Maybe<ImageWrapper> TryLoadImage(string path)
        {
            return from image in CommonFunctions.TryLoadImage(path)
                   select new ImageWrapper(image, path);
        }
    }
}
