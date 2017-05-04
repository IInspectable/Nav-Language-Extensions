#region Using Directives

using System;
using System.Collections.Generic;
using System.Linq;
using Pharmatechnik.Nav.Language.Text;

#endregion

namespace Pharmatechnik.Nav.Language.CodeFixes.Rename {

    sealed class ChoiceRenameCodeFix: RenameNodeCodeFix<IChoiceNodeSymbol> {
        
        internal ChoiceRenameCodeFix(IChoiceNodeSymbol choiceNodeSymbol, CodeGenerationUnit codeGenerationUnit, EditorSettings editorSettings) 
            : base(choiceNodeSymbol, codeGenerationUnit, editorSettings) {
        }

        public override string Name          => "Rename Choice";
        public override CodeFixImpact Impact => CodeFixImpact.None;
        IChoiceNodeSymbol ChoiceNodeSymbol   => Symbol;
        
        public override IEnumerable<TextChange> GetTextChanges(string newName) {

            newName = newName?.Trim()??String.Empty;

            var validationMessage = ValidateSymbolName(newName);
            if (!String.IsNullOrEmpty(validationMessage)) {
                throw new ArgumentException(validationMessage, nameof(newName));
            }

            var textChanges = new List<TextChange?>();
            // Die Choice Deklaration
            textChanges.Add(TryRename(ChoiceNodeSymbol, newName));

            // Die Choice-Referenzen auf der "linken Seite"
            foreach (var transition in ChoiceNodeSymbol.Outgoings) {
                var textChange = TryRenameSource(transition, newName);
                textChanges.Add(textChange);
            }

            // Die Choice-Referenzen auf der "rechten Seite"
            foreach (var transition in ChoiceNodeSymbol.Incomings) {
                var textChange = TryRenameTarget(transition, newName);
                textChanges.Add(textChange);
            }

            return textChanges.OfType<TextChange>().ToList();
        }
    }
}