#region Using Directives

using System;
using System.Collections.Generic;
using System.Linq;
using Pharmatechnik.Nav.Language.Text;

#endregion

namespace Pharmatechnik.Nav.Language.CodeFixes.Rename {

    sealed class ExitNodeRenameCodeFix : RenameCodeFix<IExitNodeSymbol> {
        
        internal ExitNodeRenameCodeFix(IExitNodeSymbol exitNodeSymbol, CodeGenerationUnit codeGenerationUnit, EditorSettings editorSettings) 
            : base(exitNodeSymbol, codeGenerationUnit, editorSettings) {
        }

        public override string Name          => "Rename Exit";
        public override CodeFixImpact Impact => CodeFixImpact.High;
        ITaskDefinitionSymbol ContainingTask => ExitNode.ContainingTask;
        IExitNodeSymbol ExitNode             => Symbol;

        public override bool CanApplyFix() {
            return true;
        }

        public override string ValidateSymbolName(string symbolName) {
            // De facto kein Rename, aber OK
            if (symbolName == ExitNode.Name) {
                return null;
            }
            return ValidateNewNodeName(symbolName, ContainingTask);            
        }
        
        public override IEnumerable<TextChange> GetTextChanges(string newAliasName) {

            if (!CanApplyFix()) {
                throw new InvalidOperationException();
            }

            newAliasName = newAliasName?.Trim()??String.Empty;

            var validationMessage = ValidateSymbolName(newAliasName);
            if (!String.IsNullOrEmpty(validationMessage)) {
                throw new ArgumentException(validationMessage, nameof(newAliasName));
            }
            
            var textChanges = new List<TextChange?>();
            // Das Exit selbst
            textChanges.Add(TryRename(ExitNode, newAliasName));

            // Die Exit-Referenzen auf der "rechten Seite"
            foreach (var transition in ExitNode.Incomings) {
                var textChange = TryRenameTarget(transition, newAliasName);
                textChanges.Add(textChange);
            }

            return textChanges.OfType<TextChange>().ToList();
        }
    }
}