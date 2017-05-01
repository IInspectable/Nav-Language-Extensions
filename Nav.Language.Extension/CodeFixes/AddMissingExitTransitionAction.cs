#region Using Directives

using System.Threading;

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Imaging.Interop;
using Pharmatechnik.Nav.Language.CodeFixes;
using Pharmatechnik.Nav.Language.Extension.Images;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.CodeFixes {

    class AddMissingExitTransitionAction : CodeFixAction<AddMissingExitTransitionCodeFix> {

        public AddMissingExitTransitionAction(AddMissingExitTransitionCodeFix codeFix,
                                              CodeFixActionsParameter parameter,
                                              CodeFixActionContext context) : base(context, parameter, codeFix) {

        }
        
        public override Span? ApplicableToSpan   => GetSnapshotSpan(CodeFix.TargetNode);
        public override ImageMoniker IconMoniker => ImageMonikers.AddEdge;
        public override string DisplayText       => $"Add mising edge for exit '{CodeFix.ConnectionPoint.Name}'";

        public override void Invoke(CancellationToken cancellationToken) {

            if (!CodeFix.CanApplyFix()) {
                return;
            }
            
            ApplyTextChanges(CodeFix.GetTextChanges());

            // TODO Selection Logik?
        }
    }
}
