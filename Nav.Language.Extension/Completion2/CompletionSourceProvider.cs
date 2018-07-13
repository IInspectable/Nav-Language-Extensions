#region Using Directives

using System.ComponentModel.Composition;

using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Utilities;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Completion2 {

    [Export(typeof(ICompletionSourceProvider))]
    [ContentType(NavLanguageContentDefinitions.ContentType)]
    [Name(CompletionProviderNames.NavCompletionSourceProvider)]
    class CompletionSourceProvider : ICompletionSourceProvider {

        private readonly NavFileCompletionCache _navFileCompletionCache;

        [ImportingConstructor]
        public CompletionSourceProvider(NavFileCompletionCache navFileCompletionCache) {
            _navFileCompletionCache = navFileCompletionCache;

        }

        public ICompletionSource TryCreateCompletionSource(ITextBuffer textBuffer) {
            return new CompletionSource(textBuffer, _navFileCompletionCache);
        }
    }
}