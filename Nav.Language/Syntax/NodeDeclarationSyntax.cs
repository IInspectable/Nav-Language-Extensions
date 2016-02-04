using System;

namespace Pharmatechnik.Nav.Language {
    [Serializable]
    public abstract class NodeDeclarationSyntax : SyntaxNode {

        protected NodeDeclarationSyntax(TextExtent extent) : base(extent) {
        }

        public SyntaxToken Semicolon {
            get { return ChildTokens().FirstOrMissing(SyntaxTokenType.Semicolon); }
        }
    }
}