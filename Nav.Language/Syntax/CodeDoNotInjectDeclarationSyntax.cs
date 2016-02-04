using System;

namespace Pharmatechnik.Nav.Language {
    [Serializable]
    [SampleSyntax("[donotinject]")]
    public partial class CodeDoNotInjectDeclarationSyntax : CodeSyntax {

        internal CodeDoNotInjectDeclarationSyntax(TextExtent extent) : base(extent) { 
        }

        public SyntaxToken DonotinjectKeyword {
            get { return ChildTokens().FirstOrMissing(SyntaxTokenType.DonotinjectKeyword); }
        }
    }
}