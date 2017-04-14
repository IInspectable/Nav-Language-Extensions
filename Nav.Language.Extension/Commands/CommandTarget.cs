#region Using Directives

using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

using Pharmatechnik.Nav.Language.Extension.Commands.Extensibility;
using Pharmatechnik.Nav.Language.Extension.Common;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Commands {

    partial class CommandTarget : IOleCommandTarget {

        readonly IWpfTextView _wpfTextView;
        readonly ICommandHandlerServiceProvider _commandHandlerServiceProvider;
        readonly IVsEditorAdaptersFactoryService _editorAdaptersFactory;

        ICommandHandlerService _commandHandlerService;
        IOleCommandTarget _nextCommandTarget;

        public CommandTarget(IWpfTextView wpfTextView,
            ICommandHandlerServiceProvider commandHandlerServiceProvider,
            IVsEditorAdaptersFactoryService editorAdaptersFactory) {

            _wpfTextView = wpfTextView;
            _commandHandlerServiceProvider = commandHandlerServiceProvider;
            _editorAdaptersFactory = editorAdaptersFactory;
        }

        public IWpfTextView WpfTextView {
            get { return _wpfTextView; }
        }

        public IOleCommandTarget NextCommandTarget {
            get { return _nextCommandTarget; }
        }

        public ICommandHandlerService HandlerService {
            get { return _commandHandlerService; }
        }

        internal void Attach() {
            var vsTextView = _editorAdaptersFactory.GetViewAdapter(_wpfTextView);
            int returnValue = vsTextView.AddCommandFilter(this, out var nextCommandTarget);
            Marshal.ThrowExceptionForHR(returnValue);

            _commandHandlerService = _commandHandlerServiceProvider.GetService(WpfTextView);
            _nextCommandTarget = nextCommandTarget;
        }

        protected virtual ITextBuffer GetSubjectBufferContainingCaret() {
            return _wpfTextView.GetBufferContainingCaret();
        }
    }
}