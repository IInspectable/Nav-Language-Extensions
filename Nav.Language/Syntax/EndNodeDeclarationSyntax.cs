using System;

namespace Pharmatechnik.Nav.Language {
    [Serializable]
    [SampleSyntax("end;")]
    public partial class EndNodeDeclarationSyntax : ConnectionPointNodeSyntax {

        internal EndNodeDeclarationSyntax(TextExtent extent)
            : base(extent) {
        }

        public SyntaxToken EndKeyword {
            get { return ChildTokens().FirstOrMissing(SyntaxTokenType.EndKeyword); }
        }
    }
}