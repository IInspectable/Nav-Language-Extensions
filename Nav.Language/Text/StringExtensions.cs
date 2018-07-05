#region Using Directives

using System;
using System.Collections.Immutable;

using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language.Text {

    static class StringExtensions {

        [NotNull]
        public static string ToCamelcase(this string s) {

            if (String.IsNullOrEmpty(s)) {
                return s ?? String.Empty;
            }

            return s.Substring(0, 1).ToLowerInvariant() + s.Substring(1);
        }

        [NotNull]
        public static string ToPascalcase(this string s) {

            if (String.IsNullOrEmpty(s)) {
                return s ?? String.Empty;
            }

            return s.Substring(0, 1).ToUpperInvariant() + s.Substring(1);
        }

        /// <summary>
        /// Liefert den Spaltenindex (beginnend bei 0) für den angegebenen Offset vom Start der Zeile. 
        /// Es werden Tabulatoren entsprechend eingerechnet.
        /// </summary>
        /// <example>
        /// Gegeben sei folgende Zeile mit gemischten Leerzeichen (o) und Tabulatoren (->) mit einer Tabulatorweite 
        /// von 4 und anschließendem Text (T). Der angeforderte Offset ist 4:
        /// TT->--->TTTTTT
        /// ^^-^---^
        /// Der Spaltenindex für den Zeichenindex 4 ist 8 (man beachte die 2 Tabulatoren!).
        /// </example>
        public static int GetColumnForOffset(this ReadOnlySpan<char> text, int tabSize, int offset) {
            var column = 0;
            for (int index = 0; index < offset; index++) {
                var c = text[index];
                if (c == '\t') {
                    column += tabSize - column % tabSize;
                } else {
                    column++;
                }
            }

            return column;
        }

        /// <summary>
        /// Liefert den Spaltenindex (beginnend bei 0) für das erste Signifikante Zeichen in der angegebenen Zeile.
        /// Als nicht signifikant gelten alle Arten von Leerzeichen. Dabei werden Tabulatoren entsprechend umgerechnet.
        /// </summary>
        /// <example>
        /// Gegeben sei folgende Zeile mit gemischten Leerzeichen (o) und Tabulatoren (->) mit einer Tabulatorweite 
        /// von 4 und anschließendem Text (T):
        /// --->oo->TTTTTT
        /// --------^ 
        /// Der Signifikante Spaltenindex für diese Zeile ist 8.
        /// </example>
        public static int GetSignificantColumn(this ReadOnlySpan<char> text, int tabSize) {
            bool hasSignificantContent = false;
            int  column                = 0;
            for (int index = 0; index < text.Length; index++) {
                var c = text[index];

                if (c == '\t') {
                    column += tabSize - column % tabSize;
                } else if (Char.IsWhiteSpace(c)) {
                    column++;
                } else {
                    hasSignificantContent = true;
                    break;
                }
            }

            return hasSignificantContent ? column : Int32.MaxValue;
        }

        public static ImmutableArray<int> ParseLineStarts(this ReadOnlySpan<char> text) {

            if (text.Length == 0) {
                return ImmutableArray.Create(0);
            }

            var lineStarts = ImmutableArray.CreateBuilder<int>();

            int index;
            int lineStart = 0;
            for (index = 0; index < text.Length; index++) {

                char c = text[index];

                bool isNewLine = false;

                if (c == '\n') {
                    isNewLine = true;
                } else if (c == '\r') {
                    isNewLine = true;
                    // => \r\n
                    if (index + 1 < text.Length && text[index + 1] == '\n') {
                        index++;
                    }
                }

                if (isNewLine) {
                    // Achtung: Extent End zeigt immer _hinter_ das letzte Zeichen!
                    var lineEnd = index + 1;
                    lineStarts.Add(lineStart);
                    lineStart = lineEnd;
                }
            }

            // Einzige/letzte Zeile nicht vergessen. 
            if (index >= lineStart) {
                lineStarts.Add(lineStart);
            }

            return lineStarts.ToImmutable();
        }

    }

}