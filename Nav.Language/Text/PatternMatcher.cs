#region Using Directives

using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace Pharmatechnik.Nav.Language.Text {

    public class PatternMatcher {

        static List<T> EmptyList<T>() => Enumerable.Empty<T>().ToList();

        public static readonly PatternMatcher Default = new PatternMatcher();

        public List<TextExtent> GetMatchedParts(string text, string pattern) {

            var textLower  = text.ToLowerInvariant();
            var typedLower = pattern.ToLowerInvariant();
            var matches    = new SortedList<int, TextExtent>();
            var match      = string.Empty;

            int startIndex = 0;

            for (int i = 0; i < typedLower.Length; i++) {
                char c = typedLower[i];

                if (!textLower.Contains(match + c)) {

                    if (!matches.Any()) {
                        return EmptyList<TextExtent>();
                    }

                    match      = string.Empty;
                    startIndex = matches.Last().Value.End;
                }

                string current = match + c;
                int    index   = textLower.IndexOf(current, startIndex, StringComparison.Ordinal);

                if (index == -1)
                    return EmptyList<TextExtent>();

                if (index > -1) {
                    matches[index] =  new TextExtent(index, current.Length);
                    match          += c;
                } else {
                    return EmptyList<TextExtent>();
                }
            }

            return matches.Values.ToList();
        }

    }

}