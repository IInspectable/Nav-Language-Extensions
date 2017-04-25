#region Using Directives

using System.Threading;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.CodeFixes {
    
    interface ICodeFixActionProvider {
        IEnumerable<ISuggestedAction> GetSuggestedActions(ImmutableList<ISymbol> symbols, CodeGenerationUnit codeGenerationUnit, ITextView textView, SnapshotSpan range, CancellationToken cancellationToken);
    }
}
