using System;

namespace Pharmatechnik.Nav.Language {
    [Serializable]
    [SampleSyntax("[generateto \"StringLiteral\"]")]
    public partial class CodeGenerateToDeclarationSyntax : CodeSyntax {

        internal CodeGenerateToDeclarationSyntax(TextExtent extent) : base(extent) {
        }

        public SyntaxToken GeneratetoKeyword {
            get { return ChildTokens().FirstOrMissing(SyntaxTokenType.GeneratetoKeyword); }
        }

        public SyntaxToken StringLiteral {
            get { return ChildTokens().FirstOrMissing(SyntaxTokenType.StringLiteral); }
        }
    }
}