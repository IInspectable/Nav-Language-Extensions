#region Using Directives

using System.ComponentModel.Composition;

using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Editor.DragDrop;
using Microsoft.VisualStudio.Utilities;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.DropHandler {

    [Export(typeof(IDropHandlerProvider))]
    [DropFormat(ClipBoardFormats.VsProjectItems)]
    [DropFormat(ClipBoardFormats.FileDrop)]
    [Name(nameof(FileDropHandlerProvider))]
    [ContentType(NavLanguageContentDefinitions.ContentType)]
    [Order(Before = "DefaultFileDropHandler")]
    class FileDropHandlerProvider: IDropHandlerProvider {

        public IDropHandler GetAssociatedDropHandler(IWpfTextView view) {
            return view.Properties.GetOrCreateSingletonProperty(() => new FileDropHandler(view));
        }

    }

}