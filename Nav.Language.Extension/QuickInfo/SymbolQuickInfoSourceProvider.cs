#region Using Directives

using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Utilities;
using Pharmatechnik.Nav.Language.Extension.Common;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.QuickInfo {

    [Export(typeof(IQuickInfoSourceProvider))]
    [Name(QuickInfoSourceProviderNames.SymbolQuickInfoSourceProvider)]
    [Order(Before = QuickInfoSourceProviderNames.DefaultQuickInfoPresenter)]
    [ContentType(NavLanguageContentDefinitions.ContentType)]
    class SymbolQuickInfoSourceProvider : IQuickInfoSourceProvider {
        
        [ImportingConstructor]
        public SymbolQuickInfoSourceProvider(ITextStructureNavigatorSelectorService navigatorService, 
                                             ITextBufferFactoryService textBufferFactoryService, 
                                             CodeContentControlProvider codeContentControlProvider,
                                             SyntaxQuickinfoBuilderService syntaxQuickinfoBuilderService) {

            SyntaxQuickinfoBuilderService = syntaxQuickinfoBuilderService;
        }

        SyntaxQuickinfoBuilderService SyntaxQuickinfoBuilderService { get; }

        public IQuickInfoSource TryCreateQuickInfoSource(ITextBuffer textBuffer) {
            return new SymbolQuickInfoSource(textBuffer, SyntaxQuickinfoBuilderService);
        }
    }
}