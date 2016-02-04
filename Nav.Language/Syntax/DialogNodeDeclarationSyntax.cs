using System;

namespace Pharmatechnik.Nav.Language {
    [Serializable]
    [SampleSyntax("dialog Identifier;")]
    public partial class DialogNodeDeclarationSyntax : NodeDeclarationSyntax {

        internal DialogNodeDeclarationSyntax(TextExtent extent): base(extent) {
        }

        public SyntaxToken DialogKeyword {
            get { return ChildTokens().FirstOrMissing(SyntaxTokenType.DialogKeyword); }
        }

        public SyntaxToken Identifier {
            get { return ChildTokens().FirstOrMissing(SyntaxTokenType.Identifier); }
        }
    }
}