#region Using Directives

using System;
using System.Collections.Generic;
using System.Linq;
using Pharmatechnik.Nav.Language.Text;

#endregion

namespace Pharmatechnik.Nav.Language.CodeFixes.Rename {

    sealed class InitAliasSymbolRenameCodeFix : SymbolRenameCodeFix {
        
        internal InitAliasSymbolRenameCodeFix(IInitNodeAliasSymbol initNodeAlias, CodeGenerationUnit codeGenerationUnit, EditorSettings editorSettings) 
            : base(initNodeAlias, codeGenerationUnit, editorSettings) {
            InitNodeAlias = initNodeAlias ?? throw new ArgumentNullException(nameof(initNodeAlias));
        }

        public override string Name          => "Rename Init Alias";
        public override CodeFixImpact Impact => CodeFixImpact.None;
        ITaskDefinitionSymbol ContainingTask => InitNodeAlias.InitNode.ContainingTask;
        IInitNodeAliasSymbol InitNodeAlias { get; }

        public override bool CanApplyFix() {
            return true;
        }

        public override string ValidateSymbolName(string symbolName) {
            // De facto kein Rename, aber OK
            if (symbolName == InitNodeAlias.Name) {
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
            // Den Init Alias
            textChanges.Add(TryRename(InitNodeAlias, newAliasName));

            // Die Choice-Referenzen auf der "linken Seite"
            foreach (var transition in InitNodeAlias.InitNode.Outgoings) {
                var textChange = TryRenameSource(transition, newAliasName);
                textChanges.Add(textChange);
            }
           
            return textChanges.OfType<TextChange>().ToList();
        }
    }
}