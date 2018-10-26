#region Using Directives

using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Editor.DragDrop;

using Pharmatechnik.Nav.Language.Extension.Commands;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.DropHandler {

    class FileDropHandler: IDropHandler {

        readonly IWpfTextView                _textView;
        readonly NavEditorOperationsProvider _navEditorOperationsProvider;

        public FileDropHandler(IWpfTextView textView, NavEditorOperationsProvider navEditorOperationsProvider) {
            _textView                    = textView;
            _navEditorOperationsProvider = navEditorOperationsProvider;
        }

        public DragDropPointerEffects HandleDataDropped(DragDropInfo dragDropInfo) {

            var pasteCommand = GetPasteNavFileCommand();

            var executed = pasteCommand.Execute(dragDropInfo.Data);
            return executed ? DragDropPointerEffects.Link : DragDropPointerEffects.None;

        }

        public void HandleDragCanceled() {
        }

        public DragDropPointerEffects HandleDragStarted(DragDropInfo dragDropInfo) {
            return DragDropPointerEffects.Link;
        }

        public DragDropPointerEffects HandleDraggingOver(DragDropInfo dragDropInfo) {

            _textView.Caret.MoveTo(dragDropInfo.VirtualBufferPosition);

            return DragDropPointerEffects.Link;
        }

        public bool IsDropEnabled(DragDropInfo dragDropInfo) {

            var pasteCommand = GetPasteNavFileCommand();

            return pasteCommand.CanExecute(dragDropInfo.Data);
        }

        PasteNavFileCommand GetPasteNavFileCommand() {

            var pasteCommand = _navEditorOperationsProvider.CreatePasteNavFileCommand(_textView);
            return pasteCommand;
        }

    }

}