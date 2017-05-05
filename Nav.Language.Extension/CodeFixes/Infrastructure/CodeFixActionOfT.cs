#region Using Directives

using System;
using System.Threading;

using Microsoft.VisualStudio.Text;
using Pharmatechnik.Nav.Language.CodeFixes;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.CodeFixes {

    abstract class CodeFixSuggestedAction<T> : CodeFixSuggestedAction where T : CodeFix {

        protected CodeFixSuggestedAction(CodeFixActionContext context, CodeFixActionsParameter parameter, T codeFix) : base(context, parameter) {
            CodeFix = codeFix ?? throw new ArgumentNullException(nameof(codeFix));
        }

        public T CodeFix { get; }
        public override string UndoDescription => CodeFix.Name;
        public sealed override Span? ApplicableToSpan => GetSnapshotSpan(CodeFix.ApplicableTo);

        public override void Invoke(CancellationToken cancellationToken) {
          
            Apply(cancellationToken);

            SemanticModelService.TryGet(Parameter.TextBuffer)?.UpdateSynchronously();
        }
    }
}