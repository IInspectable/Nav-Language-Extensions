using System;

namespace Pharmatechnik.Nav.Language {

    [Serializable]
    [SampleSyntax("[notimplemented]")]
    public partial class CodeNotImplementedDeclarationSyntax : CodeSyntax {

        internal CodeNotImplementedDeclarationSyntax(TextExtent extent) : base(extent) {
        }

        public SyntaxToken NotimplementedKeyword {
            get { return ChildTokens().FirstOrMissing(SyntaxTokenType.NotimplementedKeyword); }
        }
    }
}