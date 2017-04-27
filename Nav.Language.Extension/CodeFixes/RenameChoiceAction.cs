#region Using Directives

using System;
using System.Threading;

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;

using Pharmatechnik.Nav.Language.Extension.Images;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.CodeFixes {

    // TODO RenameChoiceCodeFix
    class RenameChoiceAction : CodeFixAction {

        public RenameChoiceAction(IChoiceNodeSymbol choiceSymbol,
                                  CodeFixActionsParameter parameter, 
                                  CodeFixActionContext context): base(context, parameter) {

            ChoiceSymbol = choiceSymbol;
        }

        public IChoiceNodeSymbol ChoiceSymbol { get; }
        public override Span? ApplicableToSpan   => GetSnapshotSpan(ChoiceSymbol);
        public override string DisplayText       => $"Rename choice '{ChoiceSymbol.Name}'";
        public override ImageMoniker IconMoniker => KnownMonikers.Rename;

        public override void Invoke(CancellationToken cancellationToken) {
            
            var declaredNames = GetDeclaredNodeNames(ChoiceSymbol.ContainingTask);
            declaredNames.Remove(ChoiceSymbol.Name); // Ist OK - pasiert halt nix
            
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
                defaultResonse: ChoiceSymbol.Name,
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
            var waitMessage     = $"Renaming choice '{ChoiceSymbol.Name}'...";
            
            ApplyTextEdits(undoDescription, waitMessage, textEdit => {

                // TODO CodeFix daraus machen und ins Nav.Language Assembly
                // Die Choice Deklaration
                ReplaceSymbol(textEdit, ChoiceSymbol, newChoiceName);

                // Die Choice-Referenzen auf der "linken Seite"
                foreach (var transition in ChoiceSymbol.Outgoings) {
                    ReplaceSymbol(textEdit, transition.Source, newChoiceName);
                }

                // Die Choice-Referenzen auf der "rechten Seite"
                foreach (var transition in ChoiceSymbol.Incomings) {
                    ReplaceSymbol(textEdit, transition.Target, newChoiceName);
                }
            });
        }        
    }
}