#region Using Directives

using System.ComponentModel.Composition;

using Microsoft.VisualStudio.Text.Operations;

using Pharmatechnik.Nav.Language.Extension.Common;
using Pharmatechnik.Nav.Language.Extension.Utilities;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.CodeFixes {

    [Export(typeof(CodeFixActionContext))]
    class CodeFixActionContext {
        
        [ImportingConstructor]
        public CodeFixActionContext(IWaitIndicator waitIndicator, 
            ITextUndoHistoryRegistry undoHistoryRegistry,
            IEditorOperationsFactoryService editorOperationsFactoryService, 
            IDialogService dialogService) {

            WaitIndicator                  = waitIndicator;
            UndoHistoryRegistry            = undoHistoryRegistry;
            EditorOperationsFactoryService = editorOperationsFactoryService;
            DialogService             = dialogService;
        }

        public IWaitIndicator WaitIndicator { get; }
        public ITextUndoHistoryRegistry UndoHistoryRegistry { get; }
        public IEditorOperationsFactoryService EditorOperationsFactoryService { get; }
        public IDialogService DialogService { get; }
    }
}