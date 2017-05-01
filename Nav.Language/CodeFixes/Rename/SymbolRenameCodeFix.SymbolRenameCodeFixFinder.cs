#region Using Directives


#endregion

namespace Pharmatechnik.Nav.Language.CodeFixes.Rename {

    public abstract partial class SymbolRenameCodeFix: CodeFix {

        sealed class SymbolRenameCodeFixFinder : SymbolVisitor<SymbolRenameCodeFix> {

            SymbolRenameCodeFixFinder(EditorSettings editorSettings, CodeGenerationUnit codeGenerationUnit) {
                EditorSettings = editorSettings;
                CodeGenerationUnit = codeGenerationUnit;
            }

            EditorSettings EditorSettings { get; }
            CodeGenerationUnit CodeGenerationUnit { get; }

            public static SymbolRenameCodeFix Find(ISymbol symbol, EditorSettings editorSettings, CodeGenerationUnit codeGenerationUnit) {
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