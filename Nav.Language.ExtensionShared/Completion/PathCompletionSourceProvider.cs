#region Using Directives

using System.ComponentModel.Composition;

using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion;
using Microsoft.VisualStudio.Utilities;

using Pharmatechnik.Nav.Language.Extension.QuickInfo;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Completion; 

[Export(typeof(IAsyncCompletionSourceProvider))]
[ContentType(NavLanguageContentDefinitions.ContentType)]
[Name(nameof(PathCompletionSourceProvider))]
class PathCompletionSourceProvider: AsyncCompletionSourceProvider {

    [ImportingConstructor]
    public PathCompletionSourceProvider(QuickinfoBuilderService quickInfoBuilderService) {
        QuickInfoBuilderService = quickInfoBuilderService;
    }

    public QuickinfoBuilderService QuickInfoBuilderService { get; }

    protected override IAsyncCompletionSource CreateCompletionSource() {
        return new PathCompletionSource(QuickInfoBuilderService);
    }

}