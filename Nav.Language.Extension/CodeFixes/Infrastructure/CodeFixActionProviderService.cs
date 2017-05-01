#region Using Directives

using System.Linq;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.Composition;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.CodeFixes {

    interface ICodeFixActionProviderService {

        IEnumerable<CodeFixSuggestedAction> GetSuggestedActions(CodeFixActionsParameter codeFixActionsParameter, CancellationToken cancellationToken);
    }

    [Export(typeof(ICodeFixActionProviderService))]
    class CodeFixActionProviderProviderService: ICodeFixActionProviderService {

        readonly ImmutableList<ICodeFixActionProvider> _codeFixActionProviders;

        [ImportingConstructor]
        public CodeFixActionProviderProviderService([ImportMany] IEnumerable<ICodeFixActionProvider> codeFixActionProviders) {
            _codeFixActionProviders = codeFixActionProviders?.ToImmutableList()??ImmutableList<ICodeFixActionProvider>.Empty;
        }
        
        public IEnumerable<CodeFixSuggestedAction> GetSuggestedActions(CodeFixActionsParameter codeFixActionsParameter, CancellationToken cancellationToken) {
            return _codeFixActionProviders.SelectMany(p=> p.GetSuggestedActions(codeFixActionsParameter, cancellationToken));
        }
    }
}