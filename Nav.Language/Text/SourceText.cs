#region Using Directives

using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;

using JetBrains.Annotations;

using Pharmatechnik.Nav.Language.Internal;

#endregion

namespace Pharmatechnik.Nav.Language.Text {

    [Serializable]
    public abstract class SourceText {

        [CanBeNull]
        public abstract FileInfo FileInfo { get; }

        [NotNull]
        public abstract string Text { get; }

        public abstract int Length { get; }

        [NotNull]
        public abstract IReadOnlyList<SourceTextLine> TextLines { get; }

        public abstract string Substring(int startIndex, int length);

        public static SourceText From(string text, string filePath = null) {
            return new StringSourceText(text: text, filePath: filePath);
        }

        public static SourceText Empty => new StringSourceText(null, null);

        public Location GetLocation(TextExtent extent) {
            return new Location(extent, GetLineRange(extent), FileInfo?.FullName);
        }
        
        public SourceTextLine GetTextLineAtPosition(int position) {
            if (position < 0 || position > Length) {
                throw new ArgumentOutOfRangeException(nameof(position));
            }

            return GetTextLineAtPositionCore(position);
        }

        LineRange GetLineRange(TextExtent extent) {

            var start = GetLinePositionAtPosition(extent.Start);
            var end   = GetLinePositionAtPosition(extent.End);

            return new LineRange(start, end);
        }

        LinePosition GetLinePositionAtPosition(int position) {
            var lineInformaton = GetTextLineAtPositionCore(position);
            return new LinePosition(lineInformaton.Line, position - lineInformaton.Extent.Start);
        }

        int _lastLineNumber;

        SourceTextLine GetTextLineAtPositionCore(int position) {
            
            if (position == 0 ) {
                return TextLines[0];
            }

            if (position == Length) {
                return TextLines[TextLines.Count - 1];
            }

            // Natürlich ist der Zugriff auf _lastLineNumber nicht "Threadsafe". Das macht aber auch nichts. Wir verwenden den Wert nur als Hint
            // da davon auszugehen ist, dass die Zugriffe auf die Zeileninformationen immer in etwa im selben Bereich stattfinden. Im worst case
            // werden ohnehin alle Zeilen durchsucht-
            var lastLineNumber = _lastLineNumber;
            if (position >= TextLines[lastLineNumber].Start)
            {
                var limit = Math.Min(TextLines.Count, lastLineNumber + 4);
                for (int i = lastLineNumber; i < limit; i++)
                {
                    if (position < TextLines[i].Start)
                    {
                        var lineNumber = i - 1;
                        _lastLineNumber = lineNumber;
                        return TextLines[lineNumber];
                    }
                }
            }

            var textLine = TextLines.FindElementAtPosition(position);
            _lastLineNumber = textLine.Line;
            return textLine;
        }

        public override string ToString() {
            return Text;
        }

        public string Substring(TextExtent textExtent) {
            return Text.Substring(startIndex: textExtent.Start, length: textExtent.Length);
        }

        protected IReadOnlyList<SourceTextLine> ParseTextLines(string text) {

            int index;
            int line      = 0;
            int lineStart = 0;
            var lines     = new List<SourceTextLine>();
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
                    lines.Add(new SourceTextLine(this, line: line, lineStart: lineStart, lineEnd: lineEnd));
                    line++;
                    lineStart = lineEnd;
                }
            }

            // Einzige/letzte Zeile nicht vergessen. 
            if (index >= lineStart) {
                var lineEnd = index ;
                lines.Add(new SourceTextLine(this, line: line, lineStart: lineStart, lineEnd: lineEnd));
            }

            return lines;
        }

    }

    [Serializable]
    sealed class StringSourceText: SourceText {

        readonly Lazy<IReadOnlyList<SourceTextLine>> _textLines;

        public StringSourceText(string text, string filePath) {

            Text     = text ?? String.Empty;
            FileInfo = String.IsNullOrEmpty(filePath) ? null : new FileInfo(filePath);

            _textLines = new Lazy<IReadOnlyList<SourceTextLine>>(() => ParseTextLines(Text), LazyThreadSafetyMode.PublicationOnly);
        }

        public override FileInfo FileInfo { get; }
        public override string   Text     { get; }
        public override int      Length   => Text.Length;

        public override IReadOnlyList<SourceTextLine> TextLines => _textLines.Value;

        public override string Substring(int startIndex, int length) {
            return Text.Substring(startIndex: startIndex, length: length);
        }

    }

}