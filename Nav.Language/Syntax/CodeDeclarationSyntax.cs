using System;
using System.Collections.Generic;

namespace Pharmatechnik.Nav.Language {
    [Serializable]
    [SampleSyntax("[code \"code goes here\"]")]
    public partial class CodeDeclarationSyntax : CodeSyntax {

        internal CodeDeclarationSyntax(TextExtent extent) : base(extent) {}

        public SyntaxToken CodeKeyword => ChildTokens().FirstOrMissing(SyntaxTokenType.CodeKeyword);

        public IEnumerable<SyntaxToken> GetGetStringLiterals() {
            return ChildTokens().OfType(SyntaxTokenType.StringLiteral);
        }
    }
}