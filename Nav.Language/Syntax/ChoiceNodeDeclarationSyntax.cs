using System;

namespace Pharmatechnik.Nav.Language {
    [Serializable]
    [SampleSyntax("choice ChoiceName;")]
    public partial class ChoiceNodeDeclarationSyntax : NodeDeclarationSyntax {

        internal ChoiceNodeDeclarationSyntax(TextExtent extent) 
            : base(extent) {
        }

        public SyntaxToken ChoiceKeyword {
            get { return ChildTokens().FirstOrMissing(SyntaxTokenType.ChoiceKeyword); }
        }

        public SyntaxToken Identifier {
            get { return ChildTokens().FirstOrMissing(SyntaxTokenType.Identifier); }
        }
    }
}