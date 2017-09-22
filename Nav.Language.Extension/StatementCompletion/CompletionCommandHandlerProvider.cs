#region Using Directives

using System.Diagnostics;
using System.ComponentModel.Composition;

using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Language.Intellisense;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.StatementCompletion {

    [Export(typeof(IVsTextViewCreationListener))]
    [ContentType(NavLanguageContentDefinitions.ContentType)]
    [TextViewRole(PredefinedTextViewRoles.Interactive)]
    class CompletionCommandHandlerProvider : IVsTextViewCreationListener {

        readonly IVsEditorAdaptersFactoryService _adaptersFactory;
        readonly ICompletionBroker _completionBroker ;

        [ImportingConstructor]
        public CompletionCommandHandlerProvider(IVsEditorAdaptersFactoryService adaptersFactory, ICompletionBroker completionBroker) {
            _adaptersFactory  = adaptersFactory;
            _completionBroker = completionBroker;
        }

        public void VsTextViewCreated(IVsTextView textViewAdapter) {

            IWpfTextView view = _adaptersFactory.GetWpfTextView(textViewAdapter);
            Debug.Assert(view != null);

            CompletionCommandHandler filter = new CompletionCommandHandler(view, _completionBroker);

            textViewAdapter.AddCommandFilter(filter, out var next);
            filter.Next = next;
        }
    }
}