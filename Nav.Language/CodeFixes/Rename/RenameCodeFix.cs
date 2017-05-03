#region Using Directives

using System;
using System.Linq;
using System.Collections.Generic;

using JetBrains.Annotations;

using Pharmatechnik.Nav.Language.Text;

#endregion

namespace Pharmatechnik.Nav.Language.CodeFixes.Rename {

    public abstract class RenameCodeFix: CodeFix {
        
        protected RenameCodeFix(CodeGenerationUnit codeGenerationUnit, EditorSettings editorSettings)
            : base(codeGenerationUnit, editorSettings) {           
        }
        
        public abstract string ProvideDefaultName();
        public abstract string ValidateSymbolName(string symbolName);
        public abstract IEnumerable<TextChange> GetTextChanges(string newName);

        [CanBeNull]
        public static RenameCodeFix GetCodeFix(ISymbol symbol, EditorSettings editorSettings, CodeGenerationUnit codeGenerationUnit) {
            // Es darf nie mehr als einen Rename CodeFix für ein und das selbe Symbol geben
            return GetCodeFixes<RenameCodeFix>(symbol, editorSettings, codeGenerationUnit).SingleOrDefault();
        }
    }

    public abstract class RenameCodeFix<T>: RenameCodeFix where T: class, ISymbol {

        protected RenameCodeFix(T symbol, CodeGenerationUnit codeGenerationUnit, EditorSettings editorSettings) 
            : base(codeGenerationUnit, editorSettings) {

            Symbol = symbol ?? throw new ArgumentNullException(nameof(symbol));
        }

        [NotNull]
        public T Symbol { get; }

        public override string ProvideDefaultName() {
            return Symbol.Name;
        }
    }
}