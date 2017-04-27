#region Using Directives

using System;
using System.Linq;
using System.Collections.Generic;
using Pharmatechnik.Nav.Language.Text;

#endregion

namespace Pharmatechnik.Nav.Language.CodeFixes {

    public sealed class RenameChoiceCodeFix: CodeFix {
        
        public RenameChoiceCodeFix(CodeGenerationUnit codeGenerationUnit, IChoiceNodeSymbol choiceNodeSymbol) : base(codeGenerationUnit) {
            ChoiceNodeSymbol = choiceNodeSymbol ?? throw new ArgumentNullException(nameof(choiceNodeSymbol));
        }

        public IChoiceNodeSymbol ChoiceNodeSymbol { get; }
        public ITaskDefinitionSymbol ContainingTask => ChoiceNodeSymbol.ContainingTask;

        public HashSet<string> GetUsedNodeNames() {
            return GetDeclaredNodeNames(ContainingTask);
        }

        public override bool CanApplyFix() {
            return true;
        }

        public IEnumerable<TextChange> GetTextChanges(string newChoiceName, EditorSettings editorSettings) {
            if (!CanApplyFix()) {
                return Enumerable.Empty<TextChange>().ToList();
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