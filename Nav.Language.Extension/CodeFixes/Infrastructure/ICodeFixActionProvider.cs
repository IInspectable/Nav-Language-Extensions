#region Using Directives

using System.Threading;
using System.Collections.Generic;

using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.CodeFixes {
    
    interface ICodeFixActionProvider {
        IEnumerable<ISuggestedAction> GetSuggestedActions(SnapshotSpan range, IEnumerable<ISymbol> symbols, CodeGenerationUnit codeGenerationUnit, CancellationToken cancellationToken);
    }
}
