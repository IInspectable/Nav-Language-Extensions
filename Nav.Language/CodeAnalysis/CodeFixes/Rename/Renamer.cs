#region Using Directives

using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language.CodeAnalysis.CodeFixes.Rename {
    public static class Renamer {
        
        [CanBeNull]
        public static SymbolRenameCodeFix TryFindRenameCodeFix(ISymbol symbol, EditorSettings editorSettings, CodeGenerationUnit codeGenerationUnit) {
            return SymbolRenameCodeFixFinder.FindRenameCodeFix(symbol, editorSettings, codeGenerationUnit);
        }

        sealed class SymbolRenameCodeFixFinder: SymbolVisitor<SymbolRenameCodeFix> {
            
            SymbolRenameCodeFixFinder(EditorSettings editorSettings, CodeGenerationUnit codeGenerationUnit) {
                EditorSettings     = editorSettings;
                CodeGenerationUnit = codeGenerationUnit;
            }

            EditorSettings EditorSettings { get; }
            CodeGenerationUnit CodeGenerationUnit { get; }

            public static SymbolRenameCodeFix FindRenameCodeFix(ISymbol symbol, EditorSettings editorSettings, CodeGenerationUnit codeGenerationUnit) {
                var finder = new SymbolRenameCodeFixFinder(editorSettings, codeGenerationUnit);
                return finder.Visit(symbol);
            }

            public override SymbolRenameCodeFix VisitChoiceNodeSymbol(IChoiceNodeSymbol choiceNodeSymbol) {
                return new ChoiceSymbolRenameCodeFix(EditorSettings, CodeGenerationUnit, choiceNodeSymbol);
            }

            public override SymbolRenameCodeFix VisitNodeReferenceSymbol(INodeReferenceSymbol nodeReferenceSymbol) {
                return nodeReferenceSymbol.Declaration == null ? DefaultVisit(nodeReferenceSymbol) : Visit(nodeReferenceSymbol.Declaration);
            }
        }
    }

}
