using System;
using System.Diagnostics;

using JetBrains.Annotations;

using Pharmatechnik.Nav.Language.Text;

namespace Pharmatechnik.Nav.Language {

    [Serializable]
    [DebuggerDisplay("{" + nameof(ToDebuggerDisplayString) + "(), nq}")]
    public struct SyntaxToken: IExtent {

        const int BitMask                = 0xFF;
        const int TypeBitShift           = 8;
        const int ClassificationBitShift = 0;

        readonly int _classificationAndType;

        internal SyntaxToken(SyntaxNode parent, SyntaxTokenType type, SyntaxTokenClassification classification, TextExtent extent) {
            Extent = extent;
            Parent = parent;

            _classificationAndType = ((int) type << TypeBitShift) | ((int) classification << ClassificationBitShift);
        }

        public static readonly SyntaxToken Missing = new SyntaxToken(null, SyntaxTokenType.Unknown, SyntaxTokenClassification.Unknown, TextExtent.Missing);
        public static readonly SyntaxToken Empty   = new SyntaxToken(null, SyntaxTokenType.Unknown, SyntaxTokenClassification.Unknown, TextExtent.Empty);

        public TextExtent Extent { get; }

        [CanBeNull]
        public Location GetLocation() {
            return SyntaxTree?.SourceText.GetLocation(Extent);
        }

        public SyntaxTokenClassification Classification => (SyntaxTokenClassification) ((_classificationAndType >> ClassificationBitShift) & BitMask);

        public SyntaxTokenType Type => (SyntaxTokenType) ((_classificationAndType >> TypeBitShift) & BitMask);

        public int  Start     => Extent.Start;
        public int  Length    => Extent.Length;
        public int  End       => Extent.End;
        public bool IsMissing => Parent == null || Extent.IsMissing;

        [CanBeNull]
        public SyntaxNode Parent { get; }

        [CanBeNull]
        public SyntaxTree SyntaxTree => Parent?.SyntaxTree;

        public SyntaxToken NextToken() {
            return SyntaxTree?.Tokens.NextOrPrevious(Parent, this, nextToken: true) ?? Missing;
        }

        public SyntaxToken NextToken(SyntaxTokenType type) {
            return SyntaxTree?.Tokens.NextOrPrevious(Parent, this, type, nextToken: true) ?? Missing;
        }

        public SyntaxToken NextToken(SyntaxTokenClassification tokenClassification) {
            return SyntaxTree?.Tokens.NextOrPrevious(Parent, this, tokenClassification, nextToken: true) ?? Missing;
        }

        public SyntaxToken PreviousToken() {
            return SyntaxTree?.Tokens.NextOrPrevious(Parent, this, nextToken: false) ?? Missing;
        }

        public SyntaxToken PreviousToken(SyntaxTokenType type) {
            return SyntaxTree?.Tokens.NextOrPrevious(Parent, this, type, nextToken: false) ?? Missing;
        }

        public SyntaxToken PreviousToken(SyntaxTokenClassification tokenClassification) {
            return SyntaxTree?.Tokens.NextOrPrevious(Parent, this, tokenClassification, nextToken: false) ?? Missing;
        }

        public override string ToString() {
            if (IsMissing) {
                return String.Empty;
            }

            return SyntaxTree?.SourceText.Substring(Start, Length) ?? String.Empty;
        }

        public string ToDebuggerDisplayString() {
            return $"{Extent} {Type} ({Classification})";
        }

    }

}