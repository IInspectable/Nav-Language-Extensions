#region Using Directives

using System;
using JetBrains.Annotations;

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

using Pharmatechnik.Nav.Language.CodeFixes;
using Pharmatechnik.Nav.Language.Extension.Common;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.CodeFixes {

    class CodeFixActionsParameter {
        
        public CodeFixActionsParameter(SyntaxNode syntaxNode, ISymbol symbol, CodeGenerationUnitAndSnapshot codeGenerationUnitAndSnapshot, ITextView textView) {
            SyntaxNode = syntaxNode;
            Symbols    = symbol;
            TextView   = textView ?? throw new ArgumentNullException(nameof(textView));
            CodeGenerationUnitAndSnapshot = codeGenerationUnitAndSnapshot ?? throw new ArgumentNullException(nameof(codeGenerationUnitAndSnapshot));
        }

        [CanBeNull]
        public SyntaxNode SyntaxNode { get; }
        [CanBeNull]
        public ISymbol Symbols { get; }
        [NotNull]
        public ITextView TextView { get; }
        [NotNull]
        public CodeGenerationUnitAndSnapshot CodeGenerationUnitAndSnapshot { get; }
        [NotNull]
        public ITextBuffer TextBuffer => CodeGenerationUnitAndSnapshot.Snapshot.TextBuffer;

        public EditorSettings GetEditorSettings() {
            return TextView.GetEditorSettings();
        }
    }
}