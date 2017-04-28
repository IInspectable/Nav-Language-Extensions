#region Using Directives

using System;
using System.Threading;

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Imaging.Interop;

using Pharmatechnik.Nav.Language.CodeFixes;
using Pharmatechnik.Nav.Language.Extension.Images;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.CodeFixes {

    class RenameChoiceAction : CodeFixAction {

        public RenameChoiceAction(RenameChoiceCodeFix codeFix,
                                  CodeFixActionsParameter parameter, 
                                  CodeFixActionContext context): base(context, parameter) {

            CodeFix = codeFix ?? throw new ArgumentNullException(nameof(codeFix));
        }

        public RenameChoiceCodeFix CodeFix { get; }
        public override Span? ApplicableToSpan   => GetSnapshotSpan(CodeFix.ChoiceNodeSymbol);
        public override string DisplayText       => $"Rename choice '{CodeFix.ChoiceNodeSymbol.Name}'";
        public override ImageMoniker IconMoniker => ImageMonikers.RenameNode;

        public override void Invoke(CancellationToken cancellationToken) {
            
            var newChoiceName = Context.DialogService.ShowInputDialog(
                promptText    : "Name:",
                title         : "Rename choice",
                defaultResonse: CodeFix.ChoiceNodeSymbol.Name,
                iconMoniker   : ImageMonikers.ChoiceNode,
                validator     : CodeFix.ValidateChoiceName
            )?.Trim();

            if (String.IsNullOrEmpty(newChoiceName)) {
                return;
            }

            ApplyTextChanges(
                undoDescription: DisplayText, 
                waitMessage    : $"Renaming choice '{CodeFix.ChoiceNodeSymbol.Name}'...", 
                textChanges    : CodeFix.GetTextChanges(newChoiceName));
        }
    }
}