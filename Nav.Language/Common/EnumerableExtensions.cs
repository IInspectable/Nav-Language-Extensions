#region Using Directives

using System.Collections.Generic;

#endregion

namespace Pharmatechnik.Nav.Language {
    static class EnumerableExtensions {

        public static IReadOnlyList<T> ToReadOnlyList<T>(this IEnumerable<T> source, int expectedCapacity) {
            var result = new List<T>(expectedCapacity);
            result.AddRange(source);
            return result;
        }        
    }
}