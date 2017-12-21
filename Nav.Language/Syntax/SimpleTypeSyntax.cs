using System;

namespace Pharmatechnik.Nav.Language {
    [Serializable]
    [SampleSyntax("Identifier")]
    public partial class SimpleTypeSyntax : CodeTypeSyntax {

        internal SimpleTypeSyntax(TextExtent extent) : base(extent) {
        }

        public SyntaxToken Identifier => ChildTokens().FirstOrMissing(SyntaxTokenType.Identifier);
    }
}