#region Using Directives

using System;
using System.Collections.Generic;
using System.Linq;
using Pharmatechnik.Nav.Language.Text;

#endregion

namespace Pharmatechnik.Nav.Language.CodeFixes.Rename {

    sealed class ChoiceRenameCodeFix: RenameCodeFix<IChoiceNodeSymbol> {
        
        internal ChoiceRenameCodeFix(IChoiceNodeSymbol choiceNodeSymbol, CodeGenerationUnit codeGenerationUnit, EditorSettings editorSettings) 
            : base(choiceNodeSymbol, codeGenerationUnit, editorSettings) {
        }

        public override string Name          => "Rename Choice";
        public override CodeFixImpact Impact => CodeFixImpact.None;
        ITaskDefinitionSymbol ContainingTask => ChoiceNodeSymbol.ContainingTask;
        IChoiceNodeSymbol ChoiceNodeSymbol   => Symbol;

        public override bool CanApplyFix() {
            return true;
        }

        public override string ValidateSymbolName(string symbolName) {
            // De facto kein Rename, aber OK
            if (symbolName == ChoiceNodeSymbol.Name) {
                return null;
            }
            return ValidateNewNodeName(symbolName, ContainingTask);            
        }
        
        public override IEnumerable<TextChange> GetTextChanges(string newChoiceName) {

            if (!CanApplyFix()) {
                throw new InvalidOperationException();
            }

            newChoiceName = newChoiceName?.Trim()??String.Empty;

            var validationMessage = ValidateSymbolName(newChoiceName);
            if (!String.IsNullOrEmpty(validationMessage)) {
                throw new ArgumentException(validationMessage, nameof(newChoiceName));
            }

            var textChanges = new List<TextChange?>();
            // Die Choice Deklaration
            textChanges.Add(TryRename(ChoiceNodeSymbol, newChoiceName));

            // Die Choice-Referenzen auf der "linken Seite"
            foreach (var transition in ChoiceNodeSymbol.Outgoings) {
                var textChange = TryRenameSource(transition, newChoiceName);
                textChanges.Add(textChange);
            }

            // Die Choice-Referenzen auf der "rechten Seite"
            foreach (var transition in ChoiceNodeSymbol.Incomings) {
                var textChange = TryRenameTarget(transition, newChoiceName);
                textChanges.Add(textChange);
            }

            return textChanges.OfType<TextChange>().ToList();
        }
    }
}