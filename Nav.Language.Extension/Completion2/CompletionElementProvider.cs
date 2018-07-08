#region Using Directives

using System.ComponentModel.Composition;
using System.Windows;

using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Utilities;

using Pharmatechnik.Nav.Language.Extension.QuickInfo;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Completion2 {

    [Export(typeof(IUIElementProvider<Completion, ICompletionSession>))]
    [Name(CompletionProviderNames.NavCompletionElementProvider)]
    [ContentType(NavLanguageContentDefinitions.ContentType)]
    class CompletionElementProvider: IUIElementProvider<Completion, ICompletionSession> {

        [ImportingConstructor]
        public CompletionElementProvider(SyntaxQuickinfoBuilderService syntaxQuickinfoBuilderService) {
            SyntaxQuickinfoBuilderService = syntaxQuickinfoBuilderService;

        }

        public SyntaxQuickinfoBuilderService SyntaxQuickinfoBuilderService { get; }

        public UIElement GetUIElement(Completion itemToRender, ICompletionSession context, UIElementType elementType) {
            if (elementType == UIElementType.Tooltip &&
                itemToRender.Properties.TryGetProperty(nameof(ISymbol), out ISymbol item)
            ) {
                return SyntaxQuickinfoBuilderService.BuildQuickInfoContent(item);
            }

            return null;
        }

    }

}