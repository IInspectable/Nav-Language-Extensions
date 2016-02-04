using System;

namespace Pharmatechnik.Nav.Language {
    [Serializable]
    [SampleSyntax("Identifier")]
    public partial class SimpleTypeSyntax : CodeTypeSyntax {

        internal SimpleTypeSyntax(TextExtent extent) : base(extent) {
        }

        public SyntaxToken Identifier {
            get { return ChildTokens().FirstOrMissing(SyntaxTokenType.Identifier);}
        }
    }
}