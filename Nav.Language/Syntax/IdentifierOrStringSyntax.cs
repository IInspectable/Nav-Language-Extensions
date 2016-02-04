using System;
using JetBrains.Annotations;

namespace Pharmatechnik.Nav.Language {

    [Serializable]   
    public abstract class IdentifierOrStringSyntax : SyntaxNode {
        internal IdentifierOrStringSyntax(TextExtent extent)
            : base(extent) {}
        
        [CanBeNull]
        public abstract string Text { get; }

        public abstract Location GetTextLocation();
    }

    [Serializable]
    [SampleSyntax("Identifier")]
    public sealed partial class IdentifierSyntax : IdentifierOrStringSyntax {
        internal IdentifierSyntax(TextExtent extent) : base(extent) {            
        }

        public override string Text { get { return Identifier.ToString(); } }

        public override Location GetTextLocation() {
            return GetLocation();
        }

        public SyntaxToken Identifier {
            get { return ChildTokens().FirstOrMissing(SyntaxTokenType.Identifier); }
        }
    }

    [Serializable]
    [SampleSyntax("\"StringLiteral\"")]
    public sealed partial class StringLiteralSyntax : IdentifierOrStringSyntax {
        internal StringLiteralSyntax(TextExtent extent) : base(extent) {            
        }

        public override string Text { get { return StringLiteral.ToString().Trim('"'); } }

        public override Location GetTextLocation() {

            if(Extent.IsEmpty || Extent.IsMissing) {
                return GetLocation();
            }
            
            var extent = TextExtent.FromBounds(Extent.Start + 1, Extent.End - 1);

            return SyntaxTree.GetLocation(extent);
        }

        public SyntaxToken StringLiteral {
            get { return ChildTokens().FirstOrMissing(SyntaxTokenType.StringLiteral); }
        }
    }
}
