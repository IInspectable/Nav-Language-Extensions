#region Using Directives

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text.Editor.OptionsExtensionMethods;
using Pharmatechnik.Nav.Language.CodeFixes;
using Pharmatechnik.Nav.Language.Extension.Common;
using Pharmatechnik.Nav.Language.Text;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.CodeFixes {

    abstract class CodeFixAction : ISuggestedAction {
        protected CodeFixAction(CodeFixActionContext context, CodeFixActionsParameter parameter) {
            Context   = context;
            Parameter = parameter;
        }

        protected CodeFixActionContext Context { get; }
        protected CodeFixActionsParameter Parameter { get; }

        public virtual Span? ApplicableToSpan { get; } = null;

        public abstract string DisplayText { get; }

        public virtual ImageMoniker IconMoniker {
            get { return default(ImageMoniker); }
        }

        public virtual string IconAutomationText {
            get { return null; }
        }

        public virtual string InputGestureText {
            get { return null; }
        }

        public virtual void Dispose() {

        }

        public bool TryGetTelemetryId(out Guid telemetryId) {
            telemetryId = Guid.Empty;
            return false;
        }

        public virtual bool HasActionSets {
            get { return false; }
        }

        public virtual Task<IEnumerable<SuggestedActionSet>> GetActionSetsAsync(CancellationToken cancellationToken) {
            return null;
        }

        public virtual bool HasPreview {
            get { return false; }
        }

        public virtual Task<object> GetPreviewAsync(CancellationToken cancellationToken) {
            return null;
        }

        public abstract void Invoke(CancellationToken cancellationToken);

        protected SnapshotSpan GetTextEditSpan(ITextEdit textEdit, Location location) {
            // Theoretisch kann es sein, das der Snapshot, auf dem die Semantic Anayse gelaufen ist,
            // nicht mit dem aktuellen Snaphot des textEdits übereinstimmt.
            // Ob es nun Sinn macht, den SnapshotSpan auf den aktuellen TextSnapshot zu transformieren,
            // oder ob es nicht eigentlich besser wäre, die ganze Aktion abzubrechen, wird die Zeit zeigen.
            var snapshotSpan = GetSnapshotSpan(location);
            var trackingSpan = Parameter.SemanticModelResult.Snapshot.CreateTrackingSpan(snapshotSpan, SpanTrackingMode.EdgeInclusive);
            var targetSpan   = trackingSpan.GetSpan(textEdit.Snapshot);
            return targetSpan;
        }

        protected EditorSettings EditorSettings {
            get {
                return new EditorSettings(
                    tabSize: Parameter.TextView.Options.GetTabSize(),
                    newLine: Parameter.TextView.Options.GetNewLineCharacter());
            }
        }

        protected SnapshotSpan GetSnapshotSpan(Location location) {
            return location.ToSnapshotSpan(Parameter.SemanticModelResult.Snapshot);
        }

        protected SnapshotSpan GetSnapshotSpan(ISymbol symbol) {
            return GetSnapshotSpan(symbol.Location);
        }

        protected void ApplyTextChanges(string undoDescription, string waitMessage, IEnumerable<TextChange> textChanges) {

            using (Context.WaitIndicator.StartWait(undoDescription, waitMessage, allowCancel: false))
            using (var undoTransaction = new TextUndoTransaction(undoDescription, Parameter.TextView, Context.UndoHistoryRegistry, Context.EditorOperationsFactoryService))
            using (var textEdit = Parameter.TextBuffer.CreateEdit()) {

                foreach (var change in textChanges) {
                    textEdit.Replace(change.Extent.Start, change.Extent.Length, change.NewText);
                }

                textEdit.Apply();

                undoTransaction.Commit();
            }
        }

        [Obsolete]
        protected void ApplyTextEdits(string undoDescription, string waitMessage, Action<ITextEdit> action) {

            using (Context.WaitIndicator.StartWait(undoDescription, waitMessage, allowCancel: false))
            using (var undoTransaction = new TextUndoTransaction(undoDescription, Parameter.TextView, Context.UndoHistoryRegistry, Context.EditorOperationsFactoryService))
            using (var textEdit = Parameter.TextBuffer.CreateEdit()) {

                action(textEdit);
  
                textEdit.Apply();

                undoTransaction.Commit();
            }
        }

        protected HashSet<string> GetDeclaredNodeNames(ITaskDefinitionSymbol taskDefinitionSymbol) {

            var declaredNodeNames = new HashSet<string>();
            if (taskDefinitionSymbol == null) {
                return declaredNodeNames;
            }

            foreach (var node in taskDefinitionSymbol.NodeDeclarations) {
                var nodeName = node.Name;
                if (!String.IsNullOrEmpty(nodeName)) {
                    declaredNodeNames.Add(nodeName);
                }
            }
            return declaredNodeNames;
        }

        [Obsolete]
        protected void ReplaceSymbol(ITextEdit textEdit, ISymbol symbol, string name) {

            if (symbol == null || symbol.Name == name) {
                return;
            }

            var replaceSpan = GetTextEditSpan(textEdit, symbol.Location);
            textEdit.Replace(replaceSpan, name);
        }
    }
}