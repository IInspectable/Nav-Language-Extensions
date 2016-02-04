using System;

namespace Pharmatechnik.Nav.Language {
    [Serializable]
    [SampleSyntax("[code \"code goes here\"]")]
    public partial class CodeDeclarationSyntax : CodeSyntax {

        internal CodeDeclarationSyntax(TextExtent extent) : base(extent) {}

        public SyntaxToken CodeKeyword {
            get { return ChildTokens().FirstOrMissing(SyntaxTokenType.CodeKeyword); }
        }

        public SyntaxToken StringLiteral {
            get { return ChildTokens().FirstOrMissing(SyntaxTokenType.StringLiteral); }
        }
    }
}