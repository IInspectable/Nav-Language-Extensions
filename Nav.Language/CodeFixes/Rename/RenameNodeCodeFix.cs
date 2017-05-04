namespace Pharmatechnik.Nav.Language.CodeFixes.Rename {

    abstract class RenameNodeCodeFix<T> : RenameCodeFix<T> where T : class, INodeSymbol {

        protected RenameNodeCodeFix(T symbol, CodeGenerationUnit codeGenerationUnit, EditorSettings editorSettings)
            : base(symbol, codeGenerationUnit, editorSettings) {
        }

        public ITaskDefinitionSymbol ContainingTask => Symbol.ContainingTask;
        
        public override string ValidateSymbolName(string symbolName) {
            // De facto kein Rename, aber OK
            if (symbolName == Symbol.Name) {
                return null;
            }
            return ContainingTask.ValidateNewNodeName(symbolName);
        }
    }
}