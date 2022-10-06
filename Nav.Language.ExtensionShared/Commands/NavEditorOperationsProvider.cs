#region Using Directives

using System.ComponentModel.Composition;

using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Commands; 

[Export(typeof(NavEditorOperationsProvider))]
class NavEditorOperationsProvider {

    readonly IEditorOperationsFactoryService _editorOperationsFactoryService;

    [ImportingConstructor]
    public NavEditorOperationsProvider(IEditorOperationsFactoryService editorOperationsFactoryService) {
        _editorOperationsFactoryService = editorOperationsFactoryService;

    }

    public PasteNavFileCommand CreatePasteNavFileCommand(ITextView textView) {
        return new PasteNavFileCommand(textView, _editorOperationsFactoryService);
    }

}