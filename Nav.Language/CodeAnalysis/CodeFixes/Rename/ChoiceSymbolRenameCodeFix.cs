#region Using Directives

using System;
using System.Linq;
using System.Collections.Generic;

using Pharmatechnik.Nav.Language.Text;

#endregion

namespace Pharmatechnik.Nav.Language.CodeAnalysis.CodeFixes.Rename {

    sealed class ChoiceSymbolRenameCodeFix: SymbolRenameCodeFix {
        
        internal ChoiceSymbolRenameCodeFix(EditorSettings editorSettings, CodeGenerationUnit codeGenerationUnit, IChoiceNodeSymbol choiceNodeSymbol) : base(editorSettings, codeGenerationUnit) {
            ChoiceNodeSymbol = choiceNodeSymbol ?? throw new ArgumentNullException(nameof(choiceNodeSymbol));
        }

        public override ISymbol Symbol => ChoiceNodeSymbol; 
        IChoiceNodeSymbol ChoiceNodeSymbol { get; }
        ITaskDefinitionSymbol ContainingTask => ChoiceNodeSymbol.ContainingTask;

        public override bool CanApplyFix() {
            return true;
        }

        public override string ValidateSymbolName(string symbolName) {

            symbolName = symbolName?.Trim();

            if (!SyntaxFacts.IsValidIdentifier(symbolName)) {
                return DiagnosticDescriptors.Semantic.Nav2000IdentifierExpected.MessageFormat;
            }

            var declaredNames = GetDeclaredNodeNames(ContainingTask);
            declaredNames.Remove(ChoiceNodeSymbol.Name); // Ist OK - pasiert halt nix

            if (declaredNames.Contains(symbolName)) {
                return String.Format(DiagnosticDescriptors.Semantic.Nav0022NodeWithName0AlreadyDeclared.MessageFormat, symbolName);
            }

            return null;
        }
        
        public override IEnumerable<TextChange> GetTextChanges(string newChoiceName) {

            if (!CanApplyFix()) {
                throw new InvalidOperationException();
            }

            newChoiceName = newChoiceName?.Trim()??String.Empty;

            var validationMessage = ValidateSymbolName(newChoiceName);
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
                if (transition.EdgeMode != null && transition.Source.Location.EndLine== transition.EdgeMode.Location.StartLine) {
                    // Find the First non-Whitespace Token after Source Edge
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