#region Using Directives

using System.Linq;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.Composition;

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text.Editor;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.CodeFixes {

    interface ICodeFixActionProviderService {

        IEnumerable<ISuggestedAction> GetSuggestedActions(ImmutableList<ISymbol> symbols, CodeGenerationUnit codeGenerationUnit, SnapshotSpan range, ITextView textView, CancellationToken cancellationToken);
    }

    [Export(typeof(ICodeFixActionProviderService))]
    class CodeFixActionProviderProviderService: ICodeFixActionProviderService {

        readonly ImmutableList<ICodeFixActionProvider> _codeFixActionProviders;

        [ImportingConstructor]
        public CodeFixActionProviderProviderService([ImportMany] IEnumerable<ICodeFixActionProvider> codeFixActionProviders) {
            _codeFixActionProviders = codeFixActionProviders?.ToImmutableList()??ImmutableList<ICodeFixActionProvider>.Empty;
        }
        
        public IEnumerable<ISuggestedAction> GetSuggestedActions(ImmutableList<ISymbol> symbols, CodeGenerationUnit codeGenerationUnit, SnapshotSpan range, ITextView textView, CancellationToken cancellationToken) {
            foreach (var suggestedAction in _codeFixActionProviders.SelectMany(p=> p.GetSuggestedActions(symbols, codeGenerationUnit, textView, range, cancellationToken))) {
                yield return suggestedAction;
            }
        }
    }
}