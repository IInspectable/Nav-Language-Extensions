#region Using Directives

using System.ComponentModel.Composition;

using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.PatternMatching;
using Microsoft.VisualStudio.Utilities;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Completion2 {

   // [Export(typeof(ICompletionSourceProvider))]
    [ContentType(NavLanguageContentDefinitions.ContentType)]
    [Name(CompletionProviderNames.NavCompletionSourceProvider)]
    class CompletionSourceProvider : ICompletionSourceProvider {

        private readonly NavFileCompletionCache _navFileCompletionCache;
        private readonly IPatternMatcherFactory _patternMatcherFactory;

        [ImportingConstructor]
        public CompletionSourceProvider(NavFileCompletionCache navFileCompletionCache, IPatternMatcherFactory patternMatcherFactory) {
            _navFileCompletionCache = navFileCompletionCache;
            _patternMatcherFactory = patternMatcherFactory;

        }

        public ICompletionSource TryCreateCompletionSource(ITextBuffer textBuffer) {
            return new CompletionSource(textBuffer, _navFileCompletionCache, _patternMatcherFactory);
        }
    }
}