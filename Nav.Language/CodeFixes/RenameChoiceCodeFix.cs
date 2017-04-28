#region Using Directives

using System;
using System.Linq;
using System.Collections.Generic;
using Pharmatechnik.Nav.Language.Text;

#endregion

namespace Pharmatechnik.Nav.Language.CodeFixes {

    public sealed class RenameChoiceCodeFix: CodeFix {
        
        public RenameChoiceCodeFix(EditorSettings editorSettings, CodeGenerationUnit codeGenerationUnit, IChoiceNodeSymbol choiceNodeSymbol) : base(editorSettings, codeGenerationUnit) {
            ChoiceNodeSymbol = choiceNodeSymbol ?? throw new ArgumentNullException(nameof(choiceNodeSymbol));
        }

        public IChoiceNodeSymbol ChoiceNodeSymbol { get; }
        public ITaskDefinitionSymbol ContainingTask => ChoiceNodeSymbol.ContainingTask;

        public override bool CanApplyFix() {
            return true;
        }

        public string ValidateChoiceName(string choiceName) {

            choiceName = choiceName?.Trim();

            if (!SyntaxFacts.IsValidIdentifier(choiceName)) {
                return DiagnosticDescriptors.Semantic.Nav2000IdentifierExpected.MessageFormat;
            }

            var declaredNames = GetDeclaredNodeNames(ContainingTask);
            declaredNames.Remove(ChoiceNodeSymbol.Name); // Ist OK - pasiert halt nix

            if (declaredNames.Contains(choiceName)) {
                return String.Format(DiagnosticDescriptors.Semantic.Nav0022NodeWithName0AlreadyDeclared.MessageFormat, choiceName);
            }

            return null;
        }

        public IEnumerable<TextChange> GetTextChanges(string newChoiceName) {

            if (!CanApplyFix()) {
                throw new InvalidOperationException();
            }

            newChoiceName = newChoiceName?.Trim();

            var validationMessage = ValidateChoiceName(newChoiceName);
            if (!String.IsNullOrEmpty(validationMessage)) {
                throw new ArgumentException(validationMessage, nameof(newChoiceName));
            }

            var textChanges = new List<TextChange?>();
            // Die Choice Deklaration
            textChanges.Add(NewReplace(ChoiceNodeSymbol, newChoiceName));

            // Die Choice-Referenzen auf der "linken Seite"
            foreach (var transition in ChoiceNodeSymbol.Outgoings) {
                textChanges.Add(NewReplace(transition.Source, newChoiceName));
            }

            // Die Choice-Referenzen auf der "rechten Seite"
            foreach (var transition in ChoiceNodeSymbol.Incomings) {
                textChanges.Add(NewReplace(transition.Target, newChoiceName));
            }

            return textChanges.OfType<TextChange>().ToList();
        }
    }
}