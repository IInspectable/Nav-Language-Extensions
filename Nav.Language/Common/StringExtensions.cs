#region Using Directives

using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language {
    static class StringExtensions {

        [NotNull]
        public static string ToCamelcase(this string s) {

            if (string.IsNullOrEmpty(s)) {
                return s ?? string.Empty;
            }

            return s.Substring(0, 1).ToLowerInvariant() + s.Substring(1);
        }

        [NotNull]
        public static string ToPascalcase(this string s) {

            if (string.IsNullOrEmpty(s)) {
                return s ?? string.Empty;
            }

            return s.Substring(0, 1).ToUpperInvariant() + s.Substring(1);
        }
    }
}