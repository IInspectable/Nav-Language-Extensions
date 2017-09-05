#region Using Directives

using System;
using System.Linq;
using System.Collections.Generic;

#endregion

namespace Pharmatechnik.Nav.Language {
    static class EnumerableExtensions {

        public static IReadOnlyList<T> ToReadOnlyList<T>(this IEnumerable<T> source, int expectedCapacity) {
            var result = new List<T>(expectedCapacity);
            result.AddRange(source);
            return result;
        }

        public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> source, Func<T, TKey> selector) {
            return source.GroupBy(selector).Select(x => x.First());
        }
    }
}