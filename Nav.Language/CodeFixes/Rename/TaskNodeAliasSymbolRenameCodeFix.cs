#region Using Directives

using System;
using System.Collections.Generic;
using System.Linq;
using Pharmatechnik.Nav.Language.Text;

#endregion

namespace Pharmatechnik.Nav.Language.CodeFixes.Rename {

    sealed class TaskNodeAliasSymbolRenameCodeFix : SymbolRenameCodeFix {
        
        internal TaskNodeAliasSymbolRenameCodeFix(ITaskNodeAliasSymbol taskNodeAlias, CodeGenerationUnit codeGenerationUnit, EditorSettings editorSettings) 
            : base(taskNodeAlias, codeGenerationUnit, editorSettings) {
            TaskNodeAlias = taskNodeAlias ?? throw new ArgumentNullException(nameof(taskNodeAlias));
        }

        public override string Name          => "Rename Task Alias";
        public override CodeFixImpact Impact => CodeFixImpact.Medium;
        ITaskDefinitionSymbol ContainingTask => TaskNodeAlias.TaskNode.ContainingTask;
        ITaskNodeAliasSymbol TaskNodeAlias { get; }

        public override bool CanApplyFix() {
            return true;
        }

        public override string ValidateSymbolName(string symbolName) {
            // De facto kein Rename, aber OK
            if (symbolName == TaskNodeAlias.Name) {
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
            // Den Task Alias
            textChanges.Add(TryRename(TaskNodeAlias, newAliasName));

            // Die Task-Referenzen auf der "linken Seite"
            foreach (var transition in TaskNodeAlias.TaskNode.Outgoings) {
                var textChange = TryRenameSource(transition, newAliasName);
                textChanges.Add(textChange);
            }

            // Die Task-Referenzen auf der "rechten Seite"
            foreach (var transition in TaskNodeAlias.TaskNode.Incomings) {
                var textChange = TryRenameTarget(transition, newAliasName);
                textChanges.Add(textChange);
            }

            return textChanges.OfType<TextChange>().ToList();
        }
    }
}