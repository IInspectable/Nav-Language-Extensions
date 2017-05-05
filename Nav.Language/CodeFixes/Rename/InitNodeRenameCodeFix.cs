#region Using Directives

using System;
using System.Collections.Generic;
using System.Linq;
using Pharmatechnik.Nav.Language.Text;

#endregion

namespace Pharmatechnik.Nav.Language.CodeFixes.Rename {

    sealed class InitNodeRenameCodeFix : RenameCodeFix<IInitNodeSymbol> {

        internal InitNodeRenameCodeFix(IInitNodeSymbol initNodeAlias, ISymbol originatingSymbol, CodeFixContext context)
            : base(initNodeAlias, originatingSymbol, context) {
        }     

        public override string Name          => "Rename Init";
        public override CodeFixImpact Impact => CodeFixImpact.None;
        ITaskDefinitionSymbol ContainingTask => InitNode.ContainingTask;
        IInitNodeSymbol InitNode             => Symbol;
        

        public override string ValidateSymbolName(string symbolName) {
            // De facto kein Rename, aber OK
            if (symbolName == InitNode.Name) {
                return null;
            }
            return ContainingTask.ValidateNewNodeName(symbolName);            
        }
        
        public override IEnumerable<TextChange> GetTextChanges(string newName) {

            newName = newName?.Trim()??String.Empty;

            var validationMessage = ValidateSymbolName(newName);
            if (!String.IsNullOrEmpty(validationMessage)) {
                throw new ArgumentException(validationMessage, nameof(newName));
            }
            
            var textChanges = new List<TextChange?>();
            
            if (InitNode.Alias != null) {
                // Alias umbenennen
                textChanges.Add(TryRename(InitNode.Alias, newName));
            }
            else {
                // Alias hinzufügen
                textChanges.Add(TryInsert(InitNode.Syntax.InitKeyword.End, $" {newName}"));
            }
            
            // Die Choice-Referenzen auf der "linken Seite"
            foreach (var transition in InitNode.Outgoings) {
                var textChange = TryRenameSource(transition, newName);
                textChanges.Add(textChange);
            }
           
            return textChanges.OfType<TextChange>().ToList();
        }
    }
}