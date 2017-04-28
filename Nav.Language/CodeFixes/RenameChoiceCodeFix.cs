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

            newChoiceName = newChoiceName?.Trim()??String.Empty;

            var validationMessage = ValidateChoiceName(newChoiceName);
            if (!String.IsNullOrEmpty(validationMessage)) {
                throw new ArgumentException(validationMessage, nameof(newChoiceName));
            }

            // TODO Whitespace Kompensation

            var textChanges = new List<TextChange?>();
            // Die Choice Deklaration
            textChanges.Add(NewReplace(ChoiceNodeSymbol, newChoiceName));

            // Die Choice-Referenzen auf der "linken Seite"
            foreach (var transition in ChoiceNodeSymbol.Outgoings) {
                if (transition.Source == null) {
                    continue;
                }

                // TODO Make pretty
                var oldSourceNode = transition.Source;
                var replaceExtent = oldSourceNode.Location.Extent;
                var replaceText   = newChoiceName;
                if (transition.EdgeMode != null) {
                    // Find teh First non-Whitespace Token after Source Edge
                    var firstNonWSpaceToken = transition.Syntax.SyntaxTree
                                                        .Tokens[TextExtent.FromBounds(oldSourceNode.End, transition.EdgeMode.End)]
                                                        .SkipWhile(token => token.Type==SyntaxTokenType.Whitespace).FirstOrDefault();
         
                    if (!firstNonWSpaceToken.IsMissing) {
                        
                        var availableSpace = oldSourceNode.Location.Length+ColumnsBetweenLocations(oldSourceNode.Location, firstNonWSpaceToken.GetLocation());

                        replaceExtent = TextExtent.FromBounds(oldSourceNode.Location.Start, firstNonWSpaceToken.Start);

                        var spaces = Math.Max(1, availableSpace - newChoiceName.Length);

                        replaceText = newChoiceName + new string(' ', spaces);
                    }
                }
                                
                textChanges.Add(new TextChange(replaceExtent, replaceText));
            }

            // Die Choice-Referenzen auf der "rechten Seite"
            foreach (var transition in ChoiceNodeSymbol.Incomings) {
                textChanges.Add(NewReplace(transition.Target, newChoiceName));
            }

            return textChanges.OfType<TextChange>().ToList();
        }
    }
}