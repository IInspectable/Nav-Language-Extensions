#region Using Directives

using System.ComponentModel.Composition;

using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion;
using Microsoft.VisualStudio.Utilities;

using Pharmatechnik.Nav.Language.Extension.QuickInfo;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Completion {

    [Export(typeof(IAsyncCompletionSourceProvider))]
    [ContentType(NavLanguageContentDefinitions.ContentType)]
    [Name(nameof(PathCompletionSourceProvider))]
    class PathCompletionSourceProvider: AsyncCompletionSourceProvider {

        [ImportingConstructor]
        public PathCompletionSourceProvider(QuickinfoBuilderService quickinfoBuilderService, NavFileProvider navFileProvider) {
            QuickinfoBuilderService = quickinfoBuilderService;
            NavFileProvider = navFileProvider;
        }

        public QuickinfoBuilderService QuickinfoBuilderService { get; }
        public NavFileProvider NavFileProvider { get; }

        protected override IAsyncCompletionSource CreateCompletionSource() {
            return new PathCompletionSource(QuickinfoBuilderService, NavFileProvider);
        }

    }

}