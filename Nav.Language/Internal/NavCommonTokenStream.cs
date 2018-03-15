using System.Collections.Generic;
using Antlr4.Runtime;

namespace Pharmatechnik.Nav.Language.Internal {

    sealed class NavCommonTokenStream : CommonTokenStream {

        public NavCommonTokenStream(ITokenSource tokenSource) : base(tokenSource) {
        }

        public NavCommonTokenStream(ITokenSource tokenSource, int channel) : base(tokenSource, channel) {
        }
       
        public IList<IToken> AllTokens => tokens;
    }   
}