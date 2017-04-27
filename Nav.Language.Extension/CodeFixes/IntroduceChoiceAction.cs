#region Using Directives

using System;
using System.Threading;

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Imaging;
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
        public override string DisplayText       => "Introduce choice";
        public override ImageMoniker IconMoniker => KnownMonikers.InsertClause;

        public override void Invoke(CancellationToken cancellationToken) {

            if (!CodeFix.CanApplyFix()) {
                return;
            }
            
            var declaredNames = CodeFix.GetUsedNodeNames();

            string Validator(string validationText) {

                validationText = validationText?.Trim();

                if (!SyntaxFacts.IsValidIdentifier(validationText)) {
                    return "Invalid identifier";
                }

                if (declaredNames.Contains(validationText)) {
                    return $"A node with the name '{validationText}' is already declared";
                }

                return null;
            }

            var choiceName = Context.InputDialogService.ShowDialog(
                promptText    : "Name:",
                title         : DisplayText,
                defaultResonse: $"Choice_{CodeFix.NodeReference.Name}",
                iconMoniker   : ImageMonikers.ChoiceNode,
                validator     : Validator
            )?.Trim();

            if (String.IsNullOrEmpty(choiceName)) {
                return;
            }

            Apply(choiceName);
        }

        void Apply(string choiceName) {

            var undoDescription = $"{DisplayText} '{choiceName}'";
            var waitMessage     = $"{undoDescription}...";
            var textChanges     = CodeFix.GetTextChanges(choiceName, EditorSettings);

            ApplyTextChanges(undoDescription, waitMessage, textChanges);
        }               
    }
}