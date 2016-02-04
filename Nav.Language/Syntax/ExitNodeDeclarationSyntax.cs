using System;

namespace Pharmatechnik.Nav.Language {
    [Serializable]
    [SampleSyntax("exit Identifier;")]
    public partial class ExitNodeDeclarationSyntax : ConnectionPointNodeSyntax {

        internal ExitNodeDeclarationSyntax(TextExtent extent)
            : base(extent) {
        }

        public SyntaxToken ExitKeyword {
            get { return ChildTokens().FirstOrMissing(SyntaxTokenType.ExitKeyword); }
        }

        public SyntaxToken Identifier {
            get { return ChildTokens().FirstOrMissing(SyntaxTokenType.Identifier); }
        }
    }
}