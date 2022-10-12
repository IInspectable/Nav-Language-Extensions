#region Using Directives

using System.Runtime.InteropServices;

using JetBrains.Annotations;

using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

using Pharmatechnik.Nav.Language.Extension.Common;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Commands;

partial class CommandTarget: IOleCommandTarget {

    public CommandTarget(IWpfTextView wpfTextView,
                         ICommandHandlerServiceProvider commandHandlerServiceProvider,
                         IVsEditorAdaptersFactoryService editorAdaptersFactory) {

        WpfTextView = wpfTextView;

        var vsTextView = editorAdaptersFactory.GetViewAdapter(WpfTextView);
        // ReSharper disable once PossibleNullReferenceException Lass krachen
        int returnValue = vsTextView.AddCommandFilter(this, out var nextCommandTarget);
        Marshal.ThrowExceptionForHR(returnValue);

        HandlerService    = commandHandlerServiceProvider.GetService(WpfTextView);
        NextCommandTarget = nextCommandTarget;
    }

    public IWpfTextView           WpfTextView       { get; }
    public IOleCommandTarget      NextCommandTarget { get; }
    public ICommandHandlerService HandlerService    { get; }

    [CanBeNull]
    protected virtual ITextBuffer GetSubjectBufferContainingCaret() {
        return WpfTextView.GetBufferContainingCaret();
    }

}