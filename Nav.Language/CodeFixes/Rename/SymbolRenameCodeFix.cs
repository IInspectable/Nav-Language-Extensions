#region Using Directives

using System.Collections.Generic;
using JetBrains.Annotations;
using Pharmatechnik.Nav.Language.Text;

#endregion

namespace Pharmatechnik.Nav.Language.CodeFixes.Rename {

    public abstract partial class SymbolRenameCodeFix: CodeFix {
        
        protected SymbolRenameCodeFix(EditorSettings editorSettings, CodeGenerationUnit codeGenerationUnit) : base(editorSettings, codeGenerationUnit) {
        }

        public abstract ISymbol Symbol { get; }
        public abstract string ValidateSymbolName(string symbolName);
        public abstract IEnumerable<TextChange> GetTextChanges(string newSymbolName);

        [CanBeNull]
        public static SymbolRenameCodeFix TryFindCodeFix(ISymbol symbol, EditorSettings editorSettings, CodeGenerationUnit codeGenerationUnit) {
            return SymbolRenameCodeFixFinder.Find(symbol, editorSettings, codeGenerationUnit);
        }
    }
}