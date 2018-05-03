#region Using Directives

using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

using Pharmatechnik.Nav.Language.Text;

#endregion

namespace Pharmatechnik.Nav.Language.Internal {

    static class SyntaxTokenFactory {

        public static SyntaxToken CreateToken(ITerminalNode node, SyntaxTokenClassification classification, SyntaxNode parent) {
            return CreateToken(node.Symbol, classification, parent);
        }

        public static SyntaxToken CreateToken(IToken t, SyntaxTokenClassification classification, SyntaxNode parent) {

            SyntaxTokenType type = (SyntaxTokenType)t.Type;
            
            var extend=TextExtentFactory.CreateExtent(t);
            return CreateToken(extend, type, classification, parent);
        }

        public static SyntaxToken CreateToken(TextExtent extend, SyntaxTokenType type, SyntaxTokenClassification classification, SyntaxNode parent) {

            if (extend.IsMissing) {
                return SyntaxToken.Missing;
            }

            var token = new SyntaxToken(parent, type, classification, extend);

            return token;
        }
    }
}