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

        public static SourceText From(string text, string filePath) {
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

        // TODO effizienter implementieren + UnitTests
        SourceTextLine GetTextLineAtPositionCore(int position) {
            var lineInformaton = TextLines.FindElementAtPosition(position);
            return lineInformaton;
        }

        public override string ToString() {
            return Text;
        }

        public string ToString(TextExtent textExtent) {
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
                    lines.Add(new SourceTextLine(this, line, TextExtent.FromBounds(lineStart, lineEnd)));
                    line++;
                    lineStart = lineEnd;
                }
            }

            // Letzte Zeile nicht vergessen. 
            if (index > lineStart) {
                // Achtung: Extent End zeigt immer _hinter_ das letzte Zeichen!
                var lineEnd = index + 1;
                lines.Add(new SourceTextLine(this, line, TextExtent.FromBounds(lineStart, lineEnd)));
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