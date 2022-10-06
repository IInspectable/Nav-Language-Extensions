#region Using Directives

using Antlr4.Runtime;

using Pharmatechnik.Nav.Language.Text;

#endregion

namespace Pharmatechnik.Nav.Language.Internal; 

static class TextExtentFactory {

    public static TextExtent CreateExtent(ParserRuleContext context) {

        return CreateExtent(context?.Start, context?.Stop);
    }
      
    public static TextExtent CreateExtent(IToken token) {
        return CreateExtent(token, token);
    }

    static TextExtent CreateExtent(IToken startToken, IToken endToken) {

        if (startToken ==null || endToken ==null) {
            return TextExtent.Missing;
        }

        if (startToken.StartIndex == -1 || endToken.StopIndex < -1) {
            return TextExtent.Missing;
        }

        var start = startToken.StartIndex;
        var end   = endToken.StopIndex + 1;
        // Warum auch immer Antlr so etwas absurdes liefert...
        if (end < start) {
            return TextExtent.Missing;
        }

        return TextExtent.FromBounds( 
            start: start,
            end  : end
        );
    }
}