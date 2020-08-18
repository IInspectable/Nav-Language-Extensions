#region Using Directives

using System.Threading;

using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.VisualStudio.Shell;

using Pharmatechnik.Nav.Language.CodeFixes.ErrorFix;
using Pharmatechnik.Nav.Language.Extension.Common;
using Pharmatechnik.Nav.Language.Extension.Images;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.CodeFixes {

    class AddMissingExitTransitionSuggestedAction : CodeFixSuggestedAction<AddMissingExitTransitionCodeFix> {

        public AddMissingExitTransitionSuggestedAction(AddMissingExitTransitionCodeFix codeFix,
                                                       CodeFixSuggestedActionParameter parameter,
                                                       CodeFixSuggestedActionContext context)
            : base(context, parameter, codeFix) {
        }

        public override ImageMoniker IconMoniker => ImageMonikers.AddEdge;
        public override string DisplayText       => $"Add missing edge for exit '{CodeFix.ConnectionPoint.Name}'";

        protected override void Apply(CancellationToken cancellationToken) {

            ApplyTextChanges(CodeFix.GetTextChanges());

            ThreadHelper.ThrowIfNotOnUIThread();

            var codeGenerationUnitAndSnapshot = SemanticModelService.TryGet(Parameter.TextBuffer)?.UpdateSynchronously();
            if(codeGenerationUnitAndSnapshot == null) {
                return;
            }

            var selection=CodeFix.TryGetSelectionAfterChanges(codeGenerationUnitAndSnapshot.CodeGenerationUnit);
            if(!selection.IsMissing) {
                Parameter.TextView.SetSelection(selection.ToSnapshotSpan(codeGenerationUnitAndSnapshot.Snapshot));
            }            
        }
    }
}