#region Using Directives

using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Utilities;
using Pharmatechnik.Nav.Language.Extension.Common;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.QuickInfo {

    [Export(typeof (IQuickInfoSourceProvider))]
    [Name(QuickInfoSourceProviderNames.DebugQuickInfoSourceProvider)]
    [Order(After = QuickInfoSourceProviderNames.DefaultQuickInfoPresenter)]
    [ContentType(NavLanguageContentDefinitions.ContentType)]
    class DebugQuickInfoSourceProvider: IQuickInfoSourceProvider {
        
        [ImportingConstructor]
        public DebugQuickInfoSourceProvider(ITextStructureNavigatorSelectorService navigatorService, ITextBufferFactoryService textBufferFactoryService, CodeContentControlProvider codeContentControlProvider) {
        }

        public IQuickInfoSource TryCreateQuickInfoSource(ITextBuffer textBuffer) {
            return new DebugQuickInfoSource(textBuffer);
        }
    }
}