#region Using Directives

using System;
using System.Linq;
using System.Collections.Generic;

using JetBrains.Annotations;
using Pharmatechnik.Nav.Language.Text;

#endregion

namespace Pharmatechnik.Nav.Language.CodeFixes.Rename {

    public abstract class SymbolRenameCodeFix: CodeFix {
        
        protected SymbolRenameCodeFix(ISymbol symbol, CodeGenerationUnit codeGenerationUnit, EditorSettings editorSettings) : base(editorSettings, codeGenerationUnit) {
            Symbol = symbol ?? throw new ArgumentNullException(nameof(symbol));
        }

        [NotNull]
        public ISymbol Symbol { get; }
        public abstract string ValidateSymbolName(string symbolName);
        public abstract IEnumerable<TextChange> GetTextChanges(string newSymbolName);

        [CanBeNull]
        public static SymbolRenameCodeFix TryFindCodeFix(ISymbol symbol, EditorSettings editorSettings, CodeGenerationUnit codeGenerationUnit) {
            // Es darf nie mehr als einen Rename CodeFix für ein und das selbe Symbol geben
            return FindCodeFixes<SymbolRenameCodeFix>(symbol, editorSettings, codeGenerationUnit).SingleOrDefault();
        }
    }
}