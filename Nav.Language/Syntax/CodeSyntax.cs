using System;
using Pharmatechnik.Nav.Language.Internal;

namespace Pharmatechnik.Nav.Language {
    [Serializable]
    public abstract class CodeSyntax : SyntaxNode {
        protected CodeSyntax(TextExtent extent) : base(extent) {            
        }

        public SyntaxToken OpenBracket => ChildTokens().FirstOrMissing(SyntaxTokenType.OpenBracket);

        [SuppressCodeSanityCheck("Der Name Keyword ist hier ausdrücklich gewollt.")]
        public SyntaxToken Keyword      => ChildTokens().FirstOrMissing(SyntaxTokenClassification.Keyword);
        public SyntaxToken CloseBracket => ChildTokens().FirstOrMissing(SyntaxTokenType.CloseBracket);
    }
}