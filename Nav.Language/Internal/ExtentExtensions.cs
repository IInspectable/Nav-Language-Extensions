using System;
using System.Collections.Generic;

namespace Pharmatechnik.Nav.Language.Internal {

    static class ExtentExtensions {

        public static TElement NextOrPreviousElement<TExtent, TElement>(this IReadOnlyList<TElement> tokens, TExtent extent, TElement currentToken, bool nextToken, TElement missing)
            where TExtent : IExtent where TElement : IExtent {

            if(extent==null  || currentToken==null) {
                return missing;
            }

            int index = FindFirstTokenIndexAtOrAfter(tokens, currentToken.Start);

            if (index < 0) {
                return missing; // eingenlich könnte man hier auch eine ArgumentException werfen...
            }

            index = nextToken ? index + 1 : index - 1;

            if (index < 0 || index >= tokens.Count) {
                return missing;
            }

            var resultToken = tokens[index];

            if (resultToken.Start < extent.Start || resultToken.End > extent.End) {
                return missing;
            }

            return resultToken;
        }

        public static IEnumerable<TElement> GetElements<TElement, TExtent>(this IReadOnlyList<TElement> tokens,
            TExtent extent, bool includeOverlapping)
            where TElement : IExtent
            where TExtent : IExtent {

            if (!includeOverlapping) {
                return GetElementsInside(tokens, extent);
            }
            else {
                return GetElementsIncludeOverlapping(tokens, extent);
            }            
        }

        static IEnumerable<TElement> GetElementsIncludeOverlapping<TElement, TExtent>(this IReadOnlyList<TElement> tokens,
            TExtent extent)
            where TElement : IExtent
            where TExtent : IExtent {

            int startIndex = tokens.FindIndexAtPosition(extent.Start);
            if (startIndex < 0) {
                startIndex = tokens.FindFirstTokenIndexAtOrAfter(extent.Start);
                if (startIndex < 0) {
                    yield break;
                }
            }

            // Sonderlocke für "Punktsuche"
            if(extent.Start == extent.End) {
                yield return tokens[startIndex];
                yield break;
            }

            for (int index = startIndex; index < tokens.Count; index++) {
                var token = tokens[index];
                if (token.Start >= extent.End) {
                    break;
                }
                yield return token;
            }
        }

        static IEnumerable<TElement> GetElementsInside<TElement, TExtent>(this IReadOnlyList<TElement> tokens, 
            TExtent extent) 
            where TElement: IExtent
            where TExtent : IExtent {

            int startIndex = tokens.FindFirstTokenIndexAtOrAfter(extent.Start);
            if (startIndex < 0) {
                yield break;
            }
            for (int index = startIndex; index < tokens.Count; index++) {
                var token = tokens[index];
                if (token.End > extent.End) {
                    break;
                }
                yield return token;
            }
        }

        public static T FindElementAtPosition<T>(this IReadOnlyList<T> tokens, int position, bool defaultIfNotFound=false) where T : IExtent {

            int index = tokens.FindIndexAtPosition(position);
            if (index < 0) {
                if(defaultIfNotFound) {
                    return default(T);
                }
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            var token = tokens[index];
            if (token.Start <= position && token.End > position) {
                return token;
            }

            if (defaultIfNotFound) {
                return default(T);
            }
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        /// <summary>
        /// Findet den Index des ersten Tokens, dessen Start größer oder gleich der angegebenen Position ist. 
        /// </summary>
        public static int FindFirstTokenIndexAtOrAfter<T>(this IReadOnlyList<T> tokens, int pos) where T : IExtent {

            // Da die Tokens nach Start Position aufsteigend sortiert sind, 
            // drängt sich eine Binärsuche geradezu auf.
            int iMin = 0;
            int iMax = tokens.Count - 1;

            while (iMin < iMax) {
                int iMid = iMin + (iMax - iMin) / 2;

                if (tokens[iMid].Start >= pos) {
                    iMax = iMid;
                } else {
                    iMin = iMid + 1;
                }
            }

            if (iMax == iMin && tokens[iMin].Start >= pos) {
                return iMin;
            }
            return -1;
        }

        /// <summary>
        /// Findet den Index des Elements, bei dem sich die angegebene Position innerhalb des Elements befindet.
        /// </summary>
        public static int FindIndexAtPosition<T>(this IReadOnlyList<T> tokens, int pos) where T: IExtent {

            // Da die Tokens nach Start Position aufsteigend sortiert sind, 
            // drängt sich eine Binärsuche geradezu auf.
            int iMin = 0;
            int iMax = tokens.Count - 1;

            while (iMin < iMax) {
                int iMid = iMin + (iMax - iMin) / 2;

                if (tokens[iMid].End <= pos) {
                    iMin = iMid+1;
                } else if (tokens[iMid].Start > pos) {
                    iMax = iMid;
                } else {
                    return iMid;
                }
            }

            if (iMax == iMin && tokens[iMin].Start <= pos && tokens[iMin].End > pos) {
                return iMin;
            }
            return -1;
        }
    }
}