#region Using Directives

using System.Collections.Generic;
using Pharmatechnik.Nav.Language.Text;

#endregion

namespace Pharmatechnik.Nav.Language.CodeAnalysis.CodeFixes.Rename {

    public abstract class SymbolRenameCodeFix: CodeFix {
        
        protected SymbolRenameCodeFix(EditorSettings editorSettings, CodeGenerationUnit codeGenerationUnit) : base(editorSettings, codeGenerationUnit) {
        }

        public abstract ISymbol Symbol { get; }
        public abstract string ValidateSymbolName(string symbolName);
        public abstract IEnumerable<TextChange> GetTextChanges(string newChoiceName);
    }
}