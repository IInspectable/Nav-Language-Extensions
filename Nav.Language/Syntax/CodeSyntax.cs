using System;
using Pharmatechnik.Nav.Language.Internal;

namespace Pharmatechnik.Nav.Language {
    [Serializable]
    public abstract class CodeSyntax : SyntaxNode {
        protected CodeSyntax(TextExtent extent) : base(extent) {            
        }

        public SyntaxToken OpenBracket {
            get { return ChildTokens().FirstOrMissing(SyntaxTokenType.OpenBracket); }
        }

        [SuppressCodeSanityCheck("Der Name Keyword ist hier ausdrücklich gewollt.")]
        public SyntaxToken Keyword {
            get { return ChildTokens().FirstOrMissing(SyntaxTokenClassification.Keyword); }
        }

        public SyntaxToken CloseBracket {
            get { return ChildTokens().FirstOrMissing(SyntaxTokenType.CloseBracket); }
        }
    }
}