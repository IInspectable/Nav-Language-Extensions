#region Using Directives

using System.Collections.Immutable;

using Antlr4.Runtime;

#endregion

namespace Pharmatechnik.Nav.Language.Internal {

    sealed class NavErrorListener: BaseErrorListener {

        readonly string                             _filePath;
        readonly ImmutableArray<Diagnostic>.Builder _diagnostics;

        public NavErrorListener(string filePath, ImmutableArray<Diagnostic>.Builder diagnostics) {
            _filePath    = filePath;
            _diagnostics = diagnostics;
        }

        public ImmutableArray<Diagnostic>.Builder Diagnostics => _diagnostics;

        public override void SyntaxError(IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e) {

            base.SyntaxError(recognizer, offendingSymbol, line, charPositionInLine, msg, e);

            var error = SyntaxErrorFactory.CreateDiagnostic(offendingSymbol, line, charPositionInLine, msg, _filePath);

            _diagnostics.Add(error);
        }

    }

}