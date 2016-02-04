#region Using Directives

using System;
using Microsoft.VisualStudio.Text;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Common {

    static class TextSnapshotLineExtensions {
        
        #region Dokumentation
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
        #endregion
        public static int GetColumnForOffset(this ITextSnapshotLine line, int tabSize, int offset) {
            var text = line.GetText();
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

        #region Dokumentation
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
        #endregion
        public static int GetSignificantColumn(this ITextSnapshotLine line, int tabSize) {
            bool hasSignificantContent = false;
            int column = 0;
            var text = line.GetText();
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

        #region Using Directives
        /// <summary>
        /// Liefert den Offset beginnend bei 0 (= Anfang der Zeile), der zum angegbenen nullbasierten
        /// Spaltenindex führt.
        /// </summary>
        /// <example>
        /// Gegeben sei folgende Zeile mit gemischten Leerzeichen (o) und Tabulatoren (->) mit einer Tabulatorweite 
        /// von 4 und anschließendem Text (T). Der angeforderte Spaltenindex ist 8:
        /// TT->--->TTTTTT
        /// ^^-^---^
        /// Der Offset für den angeforderten Spaltenindex 8 ist 4 (man beachte die 2 Tabulatoren!).
        /// </example>
        #endregion
        public static int GetOffsetForColumn(this ITextSnapshotLine line, int column, int tabSize) {
            var text = line.GetText();
            int offset = 0;
            int currentColumn = 0;
            for (int index = 0; index < text.Length; index++) {
                var c = text[index];
                if (currentColumn == column) {
                    break;
                }
                if (c == '\t') {
                    currentColumn += tabSize - currentColumn % tabSize;
                } else {
                    currentColumn++;
                }
                offset++;
            }
            return offset;
        }
    }
}