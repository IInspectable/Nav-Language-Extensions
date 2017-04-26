#region Using Directives

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.CodeFixes {

    abstract class CodeFixAction : ISuggestedAction {

        readonly CodeFixActionsArgs _codeFixActionsArgs;
        public CodeFixActionContext Context { get; }

        protected CodeFixAction(CodeFixActionContext context, CodeFixActionsArgs codeFixActionsArgs) {
            _codeFixActionsArgs = codeFixActionsArgs;
            Context = context;
        }

        public ITextView TextView         => _codeFixActionsArgs.TextView;
        public ITextBuffer TextBuffer     => _codeFixActionsArgs.TextBuffer;
        public ITextSnapshot TextSnapshot => _codeFixActionsArgs.TextSnapshot;

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
            var snapshotSpan = new SnapshotSpan(TextSnapshot, location.Start, length: location.Length);
            var trackingSpan = TextSnapshot.CreateTrackingSpan(snapshotSpan, SpanTrackingMode.EdgeInclusive);
            var replaceSpan  = trackingSpan.GetSpan(textEdit.Snapshot);
            return replaceSpan;
        }
    }
}