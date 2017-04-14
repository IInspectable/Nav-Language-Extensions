
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.TextManager.Interop;
using Pharmatechnik.Nav.Language.Extension.Commands.Extensibility;

namespace Pharmatechnik.Nav.Language.Extension.Commands
{
    [Export(typeof(IVsTextViewCreationListener))]
    [ContentType(NavLanguageContentDefinitions.ContentType)]
    [TextViewRole(PredefinedTextViewRoles.Interactive)]
    class TextViewCreationListener : IVsTextViewCreationListener
    {

        readonly ICommandHandlerServiceProvider _commandHandlerServiceProvider;
        readonly IVsEditorAdaptersFactoryService _editorAdaptersFactory;

        [ImportingConstructor]
        public TextViewCreationListener(
            IVsEditorAdaptersFactoryService editorAdaptersFactory) {
          //  _commandHandlerServiceProvider = commandHandlerServiceProvider;
            _editorAdaptersFactory = editorAdaptersFactory;

        }

        public void VsTextViewCreated(IVsTextView textView) {

            var wpfTextView = _editorAdaptersFactory.GetWpfTextView(textView);
        }

    }
}
