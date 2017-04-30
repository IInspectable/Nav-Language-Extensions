#region Using Directives

using System;
using System.Threading;

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Imaging.Interop;
using Pharmatechnik.Nav.Language.CodeFixes;
using Pharmatechnik.Nav.Language.Extension.Images;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.CodeFixes {
    
    class IntroduceChoiceAction : CodeFixAction {

        public IntroduceChoiceAction(IntroduceChoiceCodeFix codeFix,
                                     CodeFixActionsParameter parameter, 
                                     CodeFixActionContext context): base(context, parameter) {

            CodeFix = codeFix ?? throw new ArgumentNullException(nameof(codeFix));
        }

        IntroduceChoiceCodeFix CodeFix { get; }
        public override Span? ApplicableToSpan   => GetSnapshotSpan(CodeFix.NodeReference);
        public override string DisplayText       => CodeFix.DisplayText;
        public override ImageMoniker IconMoniker => ImageMonikers.InsertNode;

        public override void Invoke(CancellationToken cancellationToken) {

            if (!CodeFix.CanApplyFix()) {
                return;
            }
            
            var choiceName = Context.DialogService.ShowInputDialog(
                promptText    : "Name:",
                title         : DisplayText,
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