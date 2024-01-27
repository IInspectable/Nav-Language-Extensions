#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;

namespace Pharmatechnik.Nav.Language; 

static class EnumerableExtensions {

    public static IReadOnlyList<T> ToReadOnlyList<T>(this IEnumerable<T> source, int expectedCapacity) {
        var result = new List<T>(expectedCapacity);
        result.AddRange(source);
        return result;
    }

    public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> source, Func<T, TKey> selector) {
        return source.GroupBy(selector).Select(x => x.First());
    }

    /// <summary>
    /// Filtert aus einer Sequenz von Elementen alle Null-Objekte heraus.
    /// </summary>
    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?>? source) {
        if (source == null) {
            return Enumerable.Empty<T>();
        }

        return source.Where(t => t != null)!;
    }

    public static IEnumerable<T> AsEnumerable<T>(this T value) {
        return Enumerable.Repeat(value, 1);
    }

}