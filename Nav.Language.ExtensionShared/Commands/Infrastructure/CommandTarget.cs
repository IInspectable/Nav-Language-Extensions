#region Using Directives

using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Pharmatechnik.Nav.Language.Extension.Common;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Commands {

    partial class CommandTarget : IOleCommandTarget {

        readonly IWpfTextView _wpfTextView;
        readonly ICommandHandlerService _commandHandlerService;
        readonly IOleCommandTarget _nextCommandTarget;

        public CommandTarget(IWpfTextView wpfTextView,
            ICommandHandlerServiceProvider commandHandlerServiceProvider,
            IVsEditorAdaptersFactoryService editorAdaptersFactory) {

            _wpfTextView = wpfTextView;
            
            var vsTextView  = editorAdaptersFactory.GetViewAdapter(_wpfTextView);
            // ReSharper disable once PossibleNullReferenceException Lass krachen
            int returnValue = vsTextView.AddCommandFilter(this, out var nextCommandTarget);
            Marshal.ThrowExceptionForHR(returnValue);

            _commandHandlerService = commandHandlerServiceProvider.GetService(WpfTextView);
            _nextCommandTarget     = nextCommandTarget;
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

        [CanBeNull]
        protected virtual ITextBuffer GetSubjectBufferContainingCaret() {
            return _wpfTextView.GetBufferContainingCaret();
        }
    }
}