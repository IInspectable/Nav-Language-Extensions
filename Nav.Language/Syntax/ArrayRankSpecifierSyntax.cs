using System;

namespace Pharmatechnik.Nav.Language {
    [Serializable]
    [SampleSyntax("[]")]
    public partial class ArrayRankSpecifierSyntax : SyntaxNode {

        internal ArrayRankSpecifierSyntax(TextExtent extent) : base(extent) {
        }
     
        public SyntaxToken OpenBracket {
            get { return ChildTokens().FirstOrMissing(SyntaxTokenType.OpenBracket); }
        }

        public SyntaxToken CloseBracket {
            get { return ChildTokens().FirstOrMissing(SyntaxTokenType.CloseBracket); }
        }
    }
}