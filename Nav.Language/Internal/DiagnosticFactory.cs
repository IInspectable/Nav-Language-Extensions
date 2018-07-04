#region Using Directives

using Antlr4.Runtime;

using Pharmatechnik.Nav.Language.Text;

#endregion

namespace Pharmatechnik.Nav.Language.Internal {

    static class SyntaxErrorFactory {

        public static Diagnostic CreateDiagnostic(SourceText sourceText, IToken offendingSymbol, int line, int charPositionInLine, string msg) {

            // TODO Simplify?
            var linePosition = new LinePosition(line -1, charPositionInLine);
            var textExtent   = TextExtent.Missing;
            if (offendingSymbol != null) {
                textExtent = TextExtent.FromBounds(offendingSymbol.StartIndex, offendingSymbol.StopIndex + 1);
            }

            var location   = new Location(textExtent, linePosition, sourceText.FileInfo?.FullName);
            var diagnostic = new Diagnostic(location, DiagnosticDescriptors.NewSyntaxError(msg));

            return diagnostic;
        }

        public static Diagnostic CreateDiagnostic(SourceText sourceText, int line, int charPositionInLine, string msg) {

            // TODO Simplify?
            var textLine   = sourceText.GetTextLineAtPosition(line - 1);
            var start      = textLine.Extent.Start + charPositionInLine;
            var extent     = TextExtent.FromBounds(start, start + 1);
            var location   = sourceText.GetLocation(extent);
            var diagnostic = new Diagnostic(location, DiagnosticDescriptors.NewSyntaxError(msg));

           // Console.Error.WriteLine(diagnostic);
            return diagnostic;
        }

    }
}