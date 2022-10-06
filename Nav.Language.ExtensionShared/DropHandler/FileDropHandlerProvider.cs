#region Using Directives

using System.ComponentModel.Composition;

using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Editor.DragDrop;
using Microsoft.VisualStudio.Utilities;

using Pharmatechnik.Nav.Language.Extension.Commands;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.DropHandler; 

[Export(typeof(IDropHandlerProvider))]
[DropFormat(ClipBoardFormats.VsProjectItems)]
[DropFormat(ClipBoardFormats.FileDrop)]
[Name(nameof(FileDropHandlerProvider))]
[ContentType(NavLanguageContentDefinitions.ContentType)]
[Order(Before = "DefaultFileDropHandler")]
class FileDropHandlerProvider: IDropHandlerProvider {

    readonly NavEditorOperationsProvider _navEditorOperationsProvider;

    [ImportingConstructor]
    public FileDropHandlerProvider(NavEditorOperationsProvider navEditorOperationsProvider) {
        _navEditorOperationsProvider = navEditorOperationsProvider;

    }

    public IDropHandler GetAssociatedDropHandler(IWpfTextView view) {
        return view.Properties.GetOrCreateSingletonProperty(() => new FileDropHandler(view, _navEditorOperationsProvider));
    }

}