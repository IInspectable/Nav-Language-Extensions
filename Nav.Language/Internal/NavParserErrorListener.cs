#region Using Directives

using System;
using System.Collections.Immutable;

using Antlr4.Runtime;

using Pharmatechnik.Nav.Language.Text;

#endregion

namespace Pharmatechnik.Nav.Language.Internal {

    sealed class NavParserErrorListener: BaseErrorListener {

        public NavParserErrorListener(SourceText sourceText, ImmutableArray<Diagnostic>.Builder diagnostics) {
            SourceText  = sourceText  ?? throw new ArgumentNullException(nameof(sourceText));
            Diagnostics = diagnostics ?? throw new ArgumentNullException(nameof(diagnostics));
        }

        public SourceText                         SourceText  { get; }
        public ImmutableArray<Diagnostic>.Builder Diagnostics { get; }

        public override void SyntaxError(IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e) {

            var error = SyntaxErrorFactory.CreateDiagnostic(SourceText, offendingSymbol, line, charPositionInLine, msg);

            Diagnostics.Add(error);
        }

    }

}