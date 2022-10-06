#region Using Directives

using System.ComponentModel.Composition;

using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion;

using Pharmatechnik.Nav.Language.Extension.QuickInfo;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Completion; 

[Export(typeof(IAsyncCompletionSourceProvider))]
[ContentType(NavLanguageContentDefinitions.ContentType)]
[Name(nameof(EdgeCompletionSourceProvider))]
[Order(After = nameof(NavCompletionSourceProvider))]
class EdgeCompletionSourceProvider: AsyncCompletionSourceProvider {

    [ImportingConstructor]
    public EdgeCompletionSourceProvider(QuickinfoBuilderService quickinfoBuilderService) {
        QuickinfoBuilderService = quickinfoBuilderService;
    }

    public QuickinfoBuilderService QuickinfoBuilderService { get; }

    protected override IAsyncCompletionSource CreateCompletionSource() {
        return new EdgeCompletionSource(QuickinfoBuilderService);
    }

}