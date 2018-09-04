#region Using Directives

using System.ComponentModel.Composition;

using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion;
using Microsoft.VisualStudio.Utilities;

using Pharmatechnik.Nav.Language.Extension.QuickInfo;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Completion3 {

    [Export(typeof(IAsyncCompletionSourceProvider))]
    [ContentType(NavLanguageContentDefinitions.ContentType)]
    [Name(CompletionProviderNames.NavCompletionSourceProvider)]
    class CompletionSourceProvider: AsyncCompletionSourceProvider {

        [ImportingConstructor]
        public CompletionSourceProvider(QuickinfoBuilderService quickinfoBuilderService) {
            QuickinfoBuilderService = quickinfoBuilderService;
        }

        public QuickinfoBuilderService QuickinfoBuilderService { get; }

        protected override IAsyncCompletionSource CreateCompletionSource() {
            return new CompletionSource(QuickinfoBuilderService);
        }

    }

}