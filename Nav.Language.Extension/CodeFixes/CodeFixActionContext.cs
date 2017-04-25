using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Operations;
using Pharmatechnik.Nav.Language.Extension.Utilities;

namespace Pharmatechnik.Nav.Language.Extension.CodeFixes {
    [Export(typeof(CodeFixActionContext))]
    class CodeFixActionContext {

        public IWaitIndicator WaitIndicator { get; }
        public ITextUndoHistoryRegistry UndoHistoryRegistry { get; }
        public IEditorOperationsFactoryService EditorOperationsFactoryService { get; }

        [ImportingConstructor]
        public CodeFixActionContext(IWaitIndicator waitIndicator, 
            ITextUndoHistoryRegistry undoHistoryRegistry,
            IEditorOperationsFactoryService editorOperationsFactoryService) {
            WaitIndicator = waitIndicator;
            UndoHistoryRegistry = undoHistoryRegistry;
            EditorOperationsFactoryService = editorOperationsFactoryService;
        }
    }
}