using System;

namespace Pharmatechnik.Nav.Language {
    [Serializable]
    public abstract class CodeTypeSyntax : SyntaxNode {

        protected CodeTypeSyntax(TextExtent extent) : base(extent) {
        }
    }
}