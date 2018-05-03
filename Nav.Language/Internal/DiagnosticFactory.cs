#region Using Directives

using Antlr4.Runtime;

using Pharmatechnik.Nav.Language.Text;

#endregion

namespace Pharmatechnik.Nav.Language.Internal {

    static class SyntaxErrorFactory {

        public static Diagnostic CreateDiagnostic(IToken offendingSymbol, int line, int charPositionInLine, string msg, string filePath) {

            var linePosition = new LinePosition(line-1, charPositionInLine);
            var textExtent   = TextExtent.Missing;
            if (offendingSymbol != null) {
                textExtent = TextExtent.FromBounds(offendingSymbol.StartIndex, offendingSymbol.StopIndex + 1);
            }

            var location   = new Location(textExtent, linePosition, filePath);
            var diagnostic = new Diagnostic(location, DiagnosticDescriptors.NewSyntaxError(msg));

            return diagnostic;
        }
    }
}