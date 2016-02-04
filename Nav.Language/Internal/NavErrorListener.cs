#region Using Directives

using System.Collections.Generic;
using Antlr4.Runtime;

#endregion

namespace Pharmatechnik.Nav.Language.Internal {

    sealed class NavErrorListener : BaseErrorListener {
        readonly string _filePath;
        readonly List<Diagnostic> _diagnostics;

        public NavErrorListener(string filePath) {
            _filePath    = filePath;
            _diagnostics = new List<Diagnostic>();
        }

        public List<Diagnostic> Diagnostics {
            get { return _diagnostics; }
        }

        public override void SyntaxError(IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e) {

            base.SyntaxError(recognizer, offendingSymbol, line, charPositionInLine, msg, e);

            var error = SyntaxErrorFactory.CreateDiagnostic(offendingSymbol, line, charPositionInLine, msg, _filePath);

            _diagnostics.Add(error);
        }
    }
}