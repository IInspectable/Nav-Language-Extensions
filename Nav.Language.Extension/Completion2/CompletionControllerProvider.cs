using System.ComponentModel.Composition;
using System.Diagnostics;

using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Utilities;

namespace Pharmatechnik.Nav.Language.Extension.Completion2 {

    //[Export(typeof(IVsTextViewCreationListener))]
    [ContentType(NavLanguageContentDefinitions.ContentType)]
    [TextViewRole(PredefinedTextViewRoles.Interactive)]
    class CompletionControllerProvider: IVsTextViewCreationListener {

        readonly IVsEditorAdaptersFactoryService _adaptersFactory;
        readonly ICompletionBroker               _completionBroker;
        readonly IAsyncQuickInfoBroker           _quickInfoBroker;

        [ImportingConstructor]
        public CompletionControllerProvider(IVsEditorAdaptersFactoryService adaptersFactory,
                                            ICompletionBroker completionBroker,
                                            IAsyncQuickInfoBroker quickInfoBroker) {
            _adaptersFactory  = adaptersFactory;
            _completionBroker = completionBroker;
            _quickInfoBroker  = quickInfoBroker;
        }

        public void VsTextViewCreated(IVsTextView textViewAdapter) {

            IWpfTextView view = _adaptersFactory.GetWpfTextView(textViewAdapter);
            Debug.Assert(view != null);

            CompletionController filter = new CompletionController(
                view,
                _completionBroker,
                _quickInfoBroker);

            textViewAdapter.AddCommandFilter(filter, out var next);
            filter.Next = next;
        }

    }

}