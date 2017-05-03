#region Using Directives

using System;
using System.Collections.Generic;
using System.Linq;
using Pharmatechnik.Nav.Language.Text;

#endregion

namespace Pharmatechnik.Nav.Language.CodeFixes.Rename {

    sealed class ViewNodeRenameCodeFix : RenameNodeCodeFix<IViewNodeSymbol> {
        
        internal ViewNodeRenameCodeFix(IViewNodeSymbol viewNodeSymbol, CodeGenerationUnit codeGenerationUnit, EditorSettings editorSettings) 
            : base(viewNodeSymbol, codeGenerationUnit, editorSettings) {
        }

        public override string Name          => "Rename View Node";
        public override CodeFixImpact Impact => CodeFixImpact.High;
        IViewNodeSymbol ViewNode             => Symbol;
        
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
            textChanges.Add(TryRename(ViewNode, newName));

            // Die Dialog-Referenzen auf der "linken Seite"
            foreach (var transition in ViewNode.Outgoings) {
                var textChange = TryRenameSource(transition, newName);
                textChanges.Add(textChange);
            }

            // Die Dialog-Referenzen auf der "rechten Seite"
            foreach (var transition in ViewNode.Incomings) {
                var textChange = TryRenameTarget(transition, newName);
                textChanges.Add(textChange);
            }

            return textChanges.OfType<TextChange>().ToList();
        }
    }
}