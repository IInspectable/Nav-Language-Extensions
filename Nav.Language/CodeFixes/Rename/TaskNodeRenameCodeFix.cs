#region Using Directives

using System;
using System.Collections.Generic;
using System.Linq;
using Pharmatechnik.Nav.Language.Text;

#endregion

namespace Pharmatechnik.Nav.Language.CodeFixes.Rename {

    sealed class TaskNodeRenameCodeFix : RenameCodeFix<ITaskNodeSymbol> {
        
        internal TaskNodeRenameCodeFix(ITaskNodeSymbol taskNode, CodeGenerationUnit codeGenerationUnit, EditorSettings editorSettings) 
            : base(taskNode, codeGenerationUnit, editorSettings) {
        }

        public override string Name          => "Rename Task Node";
        public override CodeFixImpact Impact => CodeFixImpact.Medium;
        ITaskDefinitionSymbol ContainingTask => TaskNode.ContainingTask;
        ITaskNodeSymbol TaskNode             => Symbol;

        public override string ValidateSymbolName(string symbolName) {
            // De facto kein Rename, aber OK
            if (symbolName == TaskNode.Name) {
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

            if (TaskNode.Alias != null) {
                // Alias umbenennen
                textChanges.Add(TryRename(TaskNode.Alias, newName));
            } else {
                // Alias hinzufügen
                textChanges.Add(TryInsert(TaskNode.Syntax.Identifier.End, $" {newName}"));
            }

            // Die Task-Referenzen auf der "linken Seite"
            foreach (var transition in TaskNode.Outgoings) {
                var textChange = TryRenameSource(transition, newName);
                textChanges.Add(textChange);
            }

            // Die Task-Referenzen auf der "rechten Seite"
            foreach (var transition in TaskNode.Incomings) {
                var textChange = TryRenameTarget(transition, newName);
                textChanges.Add(textChange);
            }

            return textChanges.OfType<TextChange>().ToList();
        }
    }
}