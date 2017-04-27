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

    class RenameChoiceAction : CodeFixAction {

        public RenameChoiceAction(RenameChoiceCodeFix codeFix,
                                  CodeFixActionsParameter parameter, 
                                  CodeFixActionContext context): base(context, parameter) {

            CodeFix = codeFix ?? throw new ArgumentNullException(nameof(codeFix));
        }

        public RenameChoiceCodeFix CodeFix { get; }
        public override Span? ApplicableToSpan   => GetSnapshotSpan(CodeFix.ChoiceNodeSymbol);
        public override string DisplayText       => $"Rename choice '{CodeFix.ChoiceNodeSymbol.Name}'";
        public override ImageMoniker IconMoniker => KnownMonikers.Rename;

        public override void Invoke(CancellationToken cancellationToken) {

            var declaredNames = CodeFix.GetUsedNodeNames();
            declaredNames.Remove(CodeFix.ChoiceNodeSymbol.Name); // Ist OK - pasiert halt nix
            
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

            var newChoiceName = Context.InputDialogService.ShowDialog(
                promptText    : "Name:",
                title         : "Rename choice",
                defaultResonse: CodeFix.ChoiceNodeSymbol.Name,
                iconMoniker   : ImageMonikers.ChoiceNode,
                validator     : Validator
            )?.Trim();

            if (String.IsNullOrEmpty(newChoiceName)) {
                return;
            }

            Apply(newChoiceName);
        }

        void Apply(string newChoiceName) {

            var undoDescription = DisplayText;
            var waitMessage     = $"Renaming choice '{CodeFix.ChoiceNodeSymbol.Name}'...";
            var textChanges     = CodeFix.GetTextChanges(newChoiceName, GetEditorSettings());

            ApplyTextChanges(undoDescription, waitMessage, textChanges);
        }        
    }
}