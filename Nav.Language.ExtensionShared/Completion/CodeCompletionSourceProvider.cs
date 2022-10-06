#region Using Directives

using System.ComponentModel.Composition;

using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion;
using Microsoft.VisualStudio.Utilities;

using Pharmatechnik.Nav.Language.Extension.QuickInfo;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Completion; 

[Export(typeof(IAsyncCompletionSourceProvider))]
[ContentType(NavLanguageContentDefinitions.ContentType)]
[Name(nameof(CodeCompletionSourceProvider))]
class CodeCompletionSourceProvider: AsyncCompletionSourceProvider {

    [ImportingConstructor]
    public CodeCompletionSourceProvider(QuickinfoBuilderService quickinfoBuilderService) {
        QuickinfoBuilderService = quickinfoBuilderService;
    }

    public QuickinfoBuilderService QuickinfoBuilderService { get; }

    protected override IAsyncCompletionSource CreateCompletionSource() {
        return new CodeCompletionSource(QuickinfoBuilderService);
    }

}