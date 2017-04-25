#region Using Directives

using System.Linq;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.Composition;

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Language.Intellisense;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.CodeFixes {

    interface ICodeFixActionProviderService {

        IEnumerable<ISuggestedAction> GetSuggestedActions(SnapshotSpan range, IEnumerable<ISymbol> symbols, CodeGenerationUnit codeGenerationUnit, CancellationToken cancellationToken);
    }

    [Export(typeof(ICodeFixActionProviderService))]
    class CodeFixActionProviderProviderService: ICodeFixActionProviderService {

        readonly ImmutableList<ICodeFixActionProvider> _codeFixActionProviders;

        [ImportingConstructor]
        public CodeFixActionProviderProviderService([ImportMany] IEnumerable<ICodeFixActionProvider> codeFixActionProviders) {
            _codeFixActionProviders = codeFixActionProviders?.ToImmutableList()??ImmutableList<ICodeFixActionProvider>.Empty;
        }
        
        public IEnumerable<ISuggestedAction> GetSuggestedActions(SnapshotSpan range, IEnumerable<ISymbol> symbols, CodeGenerationUnit codeGenerationUnit, CancellationToken cancellationToken) {
            foreach (var suggestedAction in _codeFixActionProviders.SelectMany(p=> p.GetSuggestedActions(range, symbols, codeGenerationUnit, cancellationToken))) {
                yield return suggestedAction;
            }
        }
    }
}