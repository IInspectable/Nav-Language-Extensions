#region Using Directives

using System.Collections.Immutable;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.CodeFixes {
    internal class CodeFixActionsArgs {

        private readonly SemanticModelResult _semanticModelResult;

        public CodeFixActionsArgs(ImmutableList<ISymbol> symbolsInRange, SemanticModelResult semanticModelResult, ITextView textView, SnapshotSpan range) {
            _semanticModelResult = semanticModelResult;
            SymbolsInRange     = symbolsInRange;
            TextView           = textView;
            Range              = range;
        }

        public ITextBuffer TextBuffer => TextSnapshot.TextBuffer;
        public ITextSnapshot TextSnapshot => _semanticModelResult.Snapshot;
        public ImmutableList<ISymbol> SymbolsInRange { get; }
        public CodeGenerationUnit CodeGenerationUnit => _semanticModelResult.CodeGenerationUnit;
        public ITextView TextView { get; }
        public SnapshotSpan Range { get; }
    }
}