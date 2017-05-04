#region Using Directives

using System;
using System.Collections.Generic;
using System.Linq;
using Pharmatechnik.Nav.Language.Text;

#endregion

namespace Pharmatechnik.Nav.Language.CodeFixes.Rename {

    sealed class TaskDeclarationRenameCodeFix : RenameCodeFix<ITaskDeclarationSymbol> {
        
        internal TaskDeclarationRenameCodeFix(ITaskDeclarationSymbol taskDeclarationSymbol, CodeGenerationUnit codeGenerationUnit, EditorSettings editorSettings) 
            : base(taskDeclarationSymbol, codeGenerationUnit, editorSettings) {
        }

        public override string Name            => "Rename Task";
        public override CodeFixImpact Impact   => CodeFixImpact.High;
        ITaskDeclarationSymbol TaskDeclaration => Symbol;

        public override string ValidateSymbolName(string symbolName) {
            // De facto kein Rename, aber OK
            if (symbolName == TaskDeclaration.Name) {
                return null;
            }

            symbolName = symbolName?.Trim();

            if (!SyntaxFacts.IsValidIdentifier(symbolName)) {
                return DiagnosticDescriptors.Semantic.Nav2000IdentifierExpected.MessageFormat;
            }

            var declaredNames = CodeGenerationUnit.TaskDeclarations.Select(td=>td.Name);
            if (declaredNames.Contains(symbolName)) {
                return String.Format(DiagnosticDescriptors.Semantic.Nav0020TaskWithName0AlreadyDeclared.MessageFormat, symbolName);
            }

            return null;
        }
        
        public override IEnumerable<TextChange> GetTextChanges(string newName) {

            newName = newName?.Trim()??String.Empty;

            var validationMessage = ValidateSymbolName(newName);
            if (!String.IsNullOrEmpty(validationMessage)) {
                throw new ArgumentException(validationMessage, nameof(newName));
            }
            
            var textChanges = new List<TextChange?>();
            // Die Declaration selbst
            textChanges.Add(TryRename(TaskDeclaration, newName));

            foreach(var taskNode in TaskDeclaration.References) {

                // Die Task Node selbst
                textChanges.Add(TryRename(taskNode, newName));

                // Wenn der Knoten einen Alias hat, dann sind wir hier fertig
                if (taskNode.Alias != null) {
                    continue;
                }
                
                // Die Task-Referenzen auf der "linken Seite"
                foreach (var transition in taskNode.Outgoings) {
                    var textChange = TryRenameSource(transition, newName);
                    textChanges.Add(textChange);
                }

                // Die Task-Referenzen auf der "rechten Seite"
                foreach (var transition in taskNode.Incomings) {
                    var textChange = TryRenameTarget(transition, newName);
                    textChanges.Add(textChange);
                }
            }
            
            return textChanges.OfType<TextChange>().ToList();
        }
    }
}