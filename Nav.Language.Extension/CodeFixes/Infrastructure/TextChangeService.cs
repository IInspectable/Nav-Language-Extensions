#region Using Directives

using System.Collections.Generic;
using System.ComponentModel.Composition;

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;

using Pharmatechnik.Nav.Language.Text;
using Pharmatechnik.Nav.Language.Extension.Common;
using Pharmatechnik.Nav.Language.Extension.Utilities;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.CodeFixes {

    interface ITextChangeService {
        // TODO TextChangeAndSnapshot
        void ApplyTextChanges(ITextView textView, string undoDescription, IEnumerable<TextChange> textChanges, ITextSnapshot textChangeSnapshot);
    }
   
    [Export(typeof(ITextChangeService))]
    class TextChangeService: ITextChangeService {

        readonly IWaitIndicator _waitIndicator;
        readonly ITextUndoHistoryRegistry _undoHistoryRegistry;
        readonly IEditorOperationsFactoryService _editorOperationsFactoryService;
        
        [ImportingConstructor]
        public TextChangeService(IWaitIndicator waitIndicator,
            ITextUndoHistoryRegistry undoHistoryRegistry,
            IEditorOperationsFactoryService editorOperationsFactoryService) {

            _waitIndicator = waitIndicator;
            _undoHistoryRegistry = undoHistoryRegistry;
            _editorOperationsFactoryService = editorOperationsFactoryService;
        }

        
        // TODO TextChange With Snapshot
        public void ApplyTextChanges(ITextView textView, string undoDescription, IEnumerable<TextChange> textChanges, ITextSnapshot textChangeSnapshot) {

            var waitMessage = $"{undoDescription} in progress...";
            using (_waitIndicator.StartWait(undoDescription, waitMessage, allowCancel: false))
            using (var undoTransaction = new TextUndoTransaction(undoDescription, textView, _undoHistoryRegistry, _editorOperationsFactoryService))
            using (var textEdit = textView.TextBuffer.CreateEdit()) {

                foreach (var change in textChanges) {
                    var span = TranslateToTextEditSpan(textChangeSnapshot, change.Extent, textEdit);
                    textEdit.Replace(span, change.NewText);
                }

                textEdit.Apply();

                undoTransaction.Commit();
            }
        }

        SnapshotSpan TranslateToTextEditSpan(ITextSnapshot sourceSnapshot, TextExtent extent, ITextEdit textEdit) {
            // Theoretisch kann es sein, das der Snapshot, auf dem die Semantic Anayse gelaufen ist,
            // nicht mit dem aktuellen Snaphot des textEdits übereinstimmt.
            // Ob es nun Sinn macht, den SnapshotSpan auf den aktuellen TextSnapshot zu transformieren,
            // oder ob es nicht eigentlich besser wäre, die ganze Aktion abzubrechen, wird die Zeit zeigen.
            var snapshotSpan = GetSnapshotSpan(extent, sourceSnapshot);
            var trackingSpan = sourceSnapshot.CreateTrackingSpan(snapshotSpan, SpanTrackingMode.EdgeInclusive);
            var targetSpan = trackingSpan.GetSpan(textEdit.Snapshot);
            return targetSpan;
        }

        SnapshotSpan GetSnapshotSpan(TextExtent extent, ITextSnapshot snapshot) {
            return new SnapshotSpan(snapshot, start: extent.Start, length: extent.Length);
        }
    }
}
