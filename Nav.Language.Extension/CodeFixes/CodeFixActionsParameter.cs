#region Using Directives

using System.Collections.Immutable;

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.CodeFixes {

    class CodeFixActionsParameter {
        
        public CodeFixActionsParameter(ImmutableList<ISymbol> symbolsInRange, SemanticModelResult semanticModelResult, ITextView textView) {
            SemanticModelResult  = semanticModelResult;
            SymbolsInRange       = symbolsInRange;
            TextView             = textView;
        }

        public ImmutableList<ISymbol> SymbolsInRange { get; }
        public SemanticModelResult SemanticModelResult { get; }
        public ITextView TextView { get; }
        public ITextBuffer TextBuffer => SemanticModelResult.Snapshot.TextBuffer;
    }
}