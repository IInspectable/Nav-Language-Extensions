#region Using Directives

using System;
using System.Linq;
using System.Collections.Generic;

#endregion

namespace Pharmatechnik.Nav.Language.CodeFixes {

    sealed class AddMissingExitTransitionCodeFixProvider : SymbolVisitor<IEnumerable<AddMissingExitTransitionCodeFix>> {

        AddMissingExitTransitionCodeFixProvider(EditorSettings editorSettings, CodeGenerationUnit codeGenerationUnit) {
            EditorSettings     = editorSettings;
            CodeGenerationUnit = codeGenerationUnit;
        }

        EditorSettings EditorSettings { get; }
        CodeGenerationUnit CodeGenerationUnit { get; }

        public static IEnumerable<AddMissingExitTransitionCodeFix> TryGetCodeFixes(ISymbol symbol, CodeGenerationUnit codeGenerationUnit, EditorSettings editorSettings) {
            if (symbol == null) {
                return Enumerable.Empty<AddMissingExitTransitionCodeFix>();
            }
            var provider = new AddMissingExitTransitionCodeFixProvider(
                editorSettings     ?? throw new ArgumentNullException(nameof(editorSettings)),
                codeGenerationUnit ?? throw new ArgumentNullException(nameof(codeGenerationUnit))
            );

            return provider.Visit(symbol).Where(cf => cf.CanApplyFix());
        }

        protected override IEnumerable<AddMissingExitTransitionCodeFix> DefaultVisit(ISymbol symbol) {
            yield break;
        }

        public override IEnumerable<AddMissingExitTransitionCodeFix> VisitNodeReferenceSymbol(INodeReferenceSymbol nodeReferenceSymbol) {

            // Add Missing Edge
            var taskNode = nodeReferenceSymbol.Declaration as ITaskNodeSymbol;
            if (taskNode != null) {
                foreach (var missingExitConnectionPoint in taskNode.GetMissingExitTransitionConnectionPoints()) {
                    yield return new AddMissingExitTransitionCodeFix(nodeReferenceSymbol, missingExitConnectionPoint, CodeGenerationUnit, EditorSettings);
                }
            }
        }
    }
}