#region Using Directives

using System;
using System.Collections.Generic;
using System.Linq;
using Pharmatechnik.Nav.Language.Text;

#endregion

namespace Pharmatechnik.Nav.Language.CodeFixes.Rename {

    sealed class DialogNodeRenameCodeFix : RenameNodeCodeFix<IDialogNodeSymbol> {
        
        internal DialogNodeRenameCodeFix(IDialogNodeSymbol dialogNodeSymbol, CodeGenerationUnit codeGenerationUnit, EditorSettings editorSettings) 
            : base(dialogNodeSymbol, codeGenerationUnit, editorSettings) {
        }

        public override string Name          => "Rename Dialog Node";
        public override CodeFixImpact Impact => CodeFixImpact.High;
        IDialogNodeSymbol DialogNode         => Symbol;
        
        public override IEnumerable<TextChange> GetTextChanges(string newName) {

            if (!CanApplyFix()) {
                throw new InvalidOperationException();
            }

            newName = newName?.Trim()??String.Empty;

            var validationMessage = ValidateSymbolName(newName);
            if (!String.IsNullOrEmpty(validationMessage)) {
                throw new ArgumentException(validationMessage, nameof(newName));
            }
            
            var textChanges = new List<TextChange?>();
            // Die Dialog Node
            textChanges.Add(TryRename(DialogNode, newName));

            // Die Dialog-Referenzen auf der "linken Seite"
            foreach (var transition in DialogNode.Outgoings) {
                var textChange = TryRenameSource(transition, newName);
                textChanges.Add(textChange);
            }

            // Die Dialog-Referenzen auf der "rechten Seite"
            foreach (var transition in DialogNode.Incomings) {
                var textChange = TryRenameTarget(transition, newName);
                textChanges.Add(textChange);
            }

            return textChanges.OfType<TextChange>().ToList();
        }
    }
}