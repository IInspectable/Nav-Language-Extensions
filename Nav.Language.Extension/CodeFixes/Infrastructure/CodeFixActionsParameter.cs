#region Using Directives

using System.Collections.Immutable;

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Pharmatechnik.Nav.Language.CodeFixes;
using Pharmatechnik.Nav.Language.Extension.Common;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.CodeFixes {

    class CodeFixActionsParameter {
        
        public CodeFixActionsParameter(ImmutableList<ISymbol> symbols, CodeGenerationUnitAndSnapshot codeGenerationUnitAndSnapshot, ITextView textView) {
            CodeGenerationUnitAndSnapshot = codeGenerationUnitAndSnapshot;
            Symbols             = symbols;
            TextView            = textView;
        }
        
        public CodeGenerationUnitAndSnapshot CodeGenerationUnitAndSnapshot { get; }
        public ImmutableList<ISymbol> Symbols { get; }
        public ITextView TextView { get; }
        public ITextBuffer TextBuffer => CodeGenerationUnitAndSnapshot.Snapshot.TextBuffer;

        public EditorSettings GetEditorSettings() {
            return TextView.GetEditorSettings();
        }
    }
}