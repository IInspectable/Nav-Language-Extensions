using System;

namespace Pharmatechnik.Nav.Language {
    [Serializable]
    [SampleSyntax("view Identifier;")]
    public partial class ViewNodeDeclarationSyntax : NodeDeclarationSyntax {

        internal ViewNodeDeclarationSyntax(TextExtent extent)
            : base(extent) {
        }

        public SyntaxToken ViewKeyword {
            get { return ChildTokens().FirstOrMissing(SyntaxTokenType.ViewKeyword); }
        }

        public SyntaxToken Identifier {
            get { return ChildTokens().FirstOrMissing(SyntaxTokenType.Identifier); }
        }
    }
}