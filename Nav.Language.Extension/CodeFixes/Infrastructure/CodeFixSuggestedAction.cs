#region Using Directives

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.VisualStudio.Language.Intellisense;

using Pharmatechnik.Nav.Language.Text;
using Pharmatechnik.Nav.Language.Extension.Common;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.CodeFixes {
    
    abstract class CodeFixSuggestedAction : ISuggestedAction {

        protected CodeFixSuggestedAction(CodeFixActionContext context, CodeFixActionsParameter parameter) {
            Context   = context   ?? throw new ArgumentNullException(nameof(context));
            Parameter = parameter ?? throw new ArgumentNullException(nameof(parameter));
        }

        protected CodeFixActionContext Context { get; }
        protected CodeFixActionsParameter Parameter { get; }

        public virtual Span? ApplicableToSpan { get; } = null;

        public abstract string DisplayText { get; }
        public abstract string UndoDescription { get; }

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

        protected SnapshotSpan GetSnapshotSpan(ISymbol symbol) {
            return symbol.GetSnapshotSpan(Parameter.CodeGenerationUnitAndSnapshot.Snapshot);
        }

        protected void ApplyTextChanges(IEnumerable<TextChange> textChanges) {

            var textChangesAndSnapshot = new TextChangesAndSnapshot(
                textChanges: textChanges, 
                snapshot   : Parameter.CodeGenerationUnitAndSnapshot.Snapshot);

            Context.TextChangeService.ApplyTextChanges(
                textView              : Parameter.TextView, 
                undoDescription       : UndoDescription,
                textChangesAndSnapshot: textChangesAndSnapshot);
        }
    }   
}