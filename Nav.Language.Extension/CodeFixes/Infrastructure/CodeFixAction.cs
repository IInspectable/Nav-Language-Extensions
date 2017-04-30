#region Using Directives

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.VisualStudio.Language.Intellisense;
using Pharmatechnik.Nav.Language.Extension.Common;
using Pharmatechnik.Nav.Language.Text;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.CodeFixes {

    abstract class CodeFixAction : ISuggestedAction {

        protected CodeFixAction(CodeFixActionContext context, CodeFixActionsParameter parameter) {
            Context = context;
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

        protected SnapshotSpan GetSnapshotSpan(ISymbol symbol) {
            return symbol.GetSnapshotSpan(Parameter.SemanticModelResult.Snapshot);
        }

        protected void ApplyTextChanges(string undoDescription, string waitMessage, IEnumerable<TextChange> textChanges) {

            Context.TextChangeService.ApplyTextChanges(Parameter.TextView, undoDescription, waitMessage, textChanges, Parameter.SemanticModelResult.Snapshot);

        }
    }
}