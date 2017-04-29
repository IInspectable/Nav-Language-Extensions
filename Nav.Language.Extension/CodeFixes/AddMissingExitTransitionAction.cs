#region Using Directives

using System;
using System.Threading;

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Imaging.Interop;
using Pharmatechnik.Nav.Language.CodeAnalysis.CodeFixes;
using Pharmatechnik.Nav.Language.Extension.Images;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.CodeFixes {

    class AddMissingExitTransitionAction : CodeFixAction {

        public AddMissingExitTransitionAction(AddMissingExitTransitionCodeFix codeFix,
                                              CodeFixActionsParameter parameter,
                                              CodeFixActionContext context) : base(context, parameter) {

            CodeFix = codeFix ?? throw new ArgumentNullException(nameof(codeFix));
        }

        AddMissingExitTransitionCodeFix CodeFix { get; }

        public override Span? ApplicableToSpan => GetSnapshotSpan(CodeFix.TargetNode);
        public override string DisplayText => $"Add outgoing edge for exit '{CodeFix.ConnectionPoint.Name}";

        public override ImageMoniker IconMoniker => ImageMonikers.AddEdge;

        public override void Invoke(CancellationToken cancellationToken) {

            if (!CodeFix.CanApplyFix()) {
                return;
            }
            
            ApplyTextChanges(
                undoDescription: $"{DisplayText} '{CodeFix.ConnectionPoint.Name}'",
                waitMessage    : $"{DisplayText} '{CodeFix.ConnectionPoint.Name}'...",
                textChanges    : CodeFix.GetTextChanges());

            // TODO Selection Logik?
        }
    }
}
