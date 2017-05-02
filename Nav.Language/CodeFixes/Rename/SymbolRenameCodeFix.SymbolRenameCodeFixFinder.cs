#region Using Directives


#endregion

namespace Pharmatechnik.Nav.Language.CodeFixes.Rename {

    public abstract partial class SymbolRenameCodeFix: CodeFix {

        sealed class SymbolRenameCodeFixFinder : SymbolVisitor<SymbolRenameCodeFix> {

            SymbolRenameCodeFixFinder(ISymbol originatingSymbol, EditorSettings editorSettings, CodeGenerationUnit codeGenerationUnit) {
                OriginatingSymbol  = originatingSymbol;
                EditorSettings     = editorSettings;
                CodeGenerationUnit = codeGenerationUnit;
            }

            ISymbol OriginatingSymbol { get; }
            EditorSettings EditorSettings { get; }
            CodeGenerationUnit CodeGenerationUnit { get; }

            public static SymbolRenameCodeFix Find(ISymbol symbol, EditorSettings editorSettings, CodeGenerationUnit codeGenerationUnit) {
                var finder = new SymbolRenameCodeFixFinder(symbol, editorSettings, codeGenerationUnit);
                return finder.Visit(symbol);
            }

            public override SymbolRenameCodeFix VisitInitNodeSymbol(IInitNodeSymbol initNodeSymbol) {
                if (OriginatingSymbol == initNodeSymbol || initNodeSymbol.Alias==null) {
                    return DefaultVisit(initNodeSymbol);
                }
                return Visit(initNodeSymbol.Alias);
            }

            public override SymbolRenameCodeFix VisitInitNodeAliasSymbol(IInitNodeAliasSymbol initNodeAliasSymbol) {
                return new InitAliasSymbolRenameCodeFix(EditorSettings, CodeGenerationUnit, initNodeAliasSymbol);
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