#region Using Directives

using System;
using System.Collections.Generic;
using System.Linq;
using Pharmatechnik.Nav.Language.Text;

#endregion

namespace Pharmatechnik.Nav.Language.CodeFixes.Rename {

    sealed class InitAliasSymbolRenameCodeFix : SymbolRenameCodeFix {
        
        internal InitAliasSymbolRenameCodeFix(EditorSettings editorSettings, CodeGenerationUnit codeGenerationUnit, IInitNodeAliasSymbol initNodeAliasSymbol) : base(editorSettings, codeGenerationUnit) {
            InitNodeAlias = initNodeAliasSymbol ?? throw new ArgumentNullException(nameof(initNodeAliasSymbol));
        }

        public override string Name => "Rename Init Alias";
        public override ISymbol Symbol => InitNodeAlias;
        IInitNodeAliasSymbol InitNodeAlias { get; }
        ITaskDefinitionSymbol ContainingTask => InitNodeAlias.InitNode.ContainingTask;

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

            // TODO Whitespace Kompensation

            var textChanges = new List<TextChange?>();
            // Den Init Alias
            textChanges.Add(NewReplace(InitNodeAlias, newAliasName));

            // Die Choice-Referenzen auf der "linken Seite"
            foreach (var transition in InitNodeAlias.InitNode.Outgoings) {
                if (transition.Source == null) {
                    continue;
                }

                // TODO Make pretty Selber Code wie in ChoiceSymbolRenameCodeFix!
                var oldSourceNode = transition.Source;
                var replaceExtent = oldSourceNode.Location.Extent;
                var replaceText   = newAliasName;
                if (transition.EdgeMode != null && transition.Source.Location.EndLine== transition.EdgeMode.Location.StartLine) {
                    // Find the First non-Whitespace Token after Source Edge
                    var firstNonWSpaceToken = transition.Syntax.SyntaxTree
                                                        .Tokens[TextExtent.FromBounds(oldSourceNode.End, transition.EdgeMode.End)]
                                                        .SkipWhile(token => token.Type==SyntaxTokenType.Whitespace).FirstOrDefault();
         
                    if (!firstNonWSpaceToken.IsMissing) {
                        
                        var availableSpace = oldSourceNode.Location.Length+ColumnsBetweenLocations(oldSourceNode.Location, firstNonWSpaceToken.GetLocation());

                        replaceExtent = TextExtent.FromBounds(oldSourceNode.Location.Start, firstNonWSpaceToken.Start);

                        var spaces = Math.Max(1, availableSpace - newAliasName.Length);

                        replaceText = newAliasName + new string(' ', spaces);
                    }
                }
                                
                textChanges.Add(new TextChange(replaceExtent, replaceText));
            }
           

            return textChanges.OfType<TextChange>().ToList();
        }
    }
}