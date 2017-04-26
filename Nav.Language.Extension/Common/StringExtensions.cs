namespace Pharmatechnik.Nav.Language.Extension.Common {
    static class StringExtensions {

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
        public static int GetColumnForOffset(this string text, int tabSize, int offset) {
            var column = 0;
            for (int index = 0; index < offset; index++) {
                var c = text[index];
                if (c == '\t') {
                    column += tabSize - column % tabSize;
                }
                else {
                    column++;
                }
            }
            return column;
        }
    }
}
