#region Using Directives

using System;
using System.Threading;

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Imaging.Interop;
using Pharmatechnik.Nav.Language.CodeFixes;
using Pharmatechnik.Nav.Language.Extension.Images;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.CodeFixes {
    
    class IntroduceChoiceSuggestedAction : CodeFixSuggestedAction<IntroduceChoiceCodeFix> {

        public IntroduceChoiceSuggestedAction(IntroduceChoiceCodeFix codeFix,
                                              CodeFixActionsParameter parameter, 
                                              CodeFixActionContext context): base(context, parameter, codeFix) {
        }

        public override Span? ApplicableToSpan   => GetSnapshotSpan(CodeFix.NodeReference);
        public override ImageMoniker IconMoniker => ImageMonikers.InsertNode;
        public override string DisplayText       => "Introduce Choice";

        protected override void Apply(CancellationToken cancellationToken) {
          
            var choiceName = Context.DialogService.ShowInputDialog(
                promptText    : "Name:",
                title         : CodeFix.Name,
                defaultResonse: CodeFix.SuggestChoiceName(),
                iconMoniker   : ImageMonikers.ChoiceNode,
                validator     : CodeFix.ValidateChoiceName
            )?.Trim();

            if (String.IsNullOrEmpty(choiceName)) {
                return;
            }

            ApplyTextChanges(CodeFix.GetTextChanges(choiceName));
        }          
    }
}