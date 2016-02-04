using System;

namespace Pharmatechnik.Nav.Language {
    [Serializable]
    [SampleSyntax("[abstractmethod]")]
    public partial class CodeAbstractMethodDeclarationSyntax : CodeSyntax {

        internal CodeAbstractMethodDeclarationSyntax(TextExtent extent) : base(extent) {
        }

        public SyntaxToken AbstractmethodKeyword {
            get { return ChildTokens().FirstOrMissing(SyntaxTokenType.AbstractmethodKeyword); }
        }
    }
}