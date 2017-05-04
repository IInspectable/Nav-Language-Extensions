#region Using Directives

using System.Collections.Immutable;
using JetBrains.Annotations;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Pharmatechnik.Nav.Language.CodeFixes;
using Pharmatechnik.Nav.Language.Extension.Common;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.CodeFixes {

    class CodeFixActionsParameter {
        
        public CodeFixActionsParameter(SyntaxNode syntaxNode, ImmutableList<ISymbol> symbols, CodeGenerationUnitAndSnapshot codeGenerationUnitAndSnapshot, ITextView textView) {
            SyntaxNode = syntaxNode;
            Symbols    = symbols;
            TextView   = textView;
            CodeGenerationUnitAndSnapshot = codeGenerationUnitAndSnapshot;
        }

        [CanBeNull]
        public SyntaxNode SyntaxNode { get; }
        public CodeGenerationUnitAndSnapshot CodeGenerationUnitAndSnapshot { get; }
        public ImmutableList<ISymbol> Symbols { get; }
        public ITextView TextView { get; }
        public ITextBuffer TextBuffer => CodeGenerationUnitAndSnapshot.Snapshot.TextBuffer;

        public EditorSettings GetEditorSettings() {
            return TextView.GetEditorSettings();
        }
    }
}