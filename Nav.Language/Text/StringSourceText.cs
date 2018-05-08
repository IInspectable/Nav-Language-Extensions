#region Using Directives

using System;
using System.IO;
using System.Threading;
using System.Collections.Immutable;

#endregion

namespace Pharmatechnik.Nav.Language.Text {

    sealed class StringSourceText: SourceText {

        readonly Lazy<ImmutableArray<int>> _textLines;

        public StringSourceText(string text, string filePath) {

            Text      = text ?? String.Empty;
            FileInfo  = String.IsNullOrEmpty(filePath) ? null : new FileInfo(filePath);
            TextLines = new StringTextLineList(this);

            _textLines = new Lazy<ImmutableArray<int>>(() => Text.ParseLineStarts(), LazyThreadSafetyMode.PublicationOnly);
        }

        public override FileInfo           FileInfo  { get; }
        public override SourceTextLineList TextLines { get; }
        public override string             Text      { get; }
        public override int                Length    => Text.Length;

        public override string Substring(int startIndex, int length) {
            return Text.Substring(startIndex: startIndex, length: length);
        }

        SourceTextLine GetTextLine(int line, ImmutableArray<int> lineStarts) {

            if (line < 0 || line >= lineStarts.Length) {
                throw new ArgumentOutOfRangeException(nameof(line));
            }

            int start = lineStarts[line];
            if (line == lineStarts.Length - 1) {
                int end = Length;
                return new SourceTextLine(this, line: line, lineStart: start, lineEnd: end);
            } else {
                int end = lineStarts[line + 1];
                return new SourceTextLine(this, line: line, lineStart: start, lineEnd: end);
            }
        }

        sealed class StringTextLineList: SourceTextLineList {

            private readonly StringSourceText _sourceText;

            public StringTextLineList(StringSourceText sourceText) {
                _sourceText = sourceText;

            }

            public override int Count => _sourceText._textLines.Value.Length;

            public override SourceTextLine this[int index] => _sourceText.GetTextLine(index, _sourceText._textLines.Value);

        }

    }

}