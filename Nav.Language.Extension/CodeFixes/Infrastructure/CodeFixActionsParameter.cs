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
        
        public CodeFixActionsParameter(SnapshotPoint caretPoint, SyntaxNode originatingNode, ISymbol originatingSymbol, CodeGenerationUnitAndSnapshot codeGenerationUnitAndSnapshot, ITextView textView) {
            CaretPoint         = caretPoint;
            OriginatingNode    = originatingNode;
            OriginatingSymbol  = originatingSymbol;
            TextView           = textView ?? throw new ArgumentNullException(nameof(textView));
            CodeGenerationUnitAndSnapshot = codeGenerationUnitAndSnapshot ?? throw new ArgumentNullException(nameof(codeGenerationUnitAndSnapshot));           
        }

        #region TODO Unused Stuff?
        // TODO Unused Stuff?
        SnapshotPoint CaretPoint { get; }

        [CanBeNull]
        public SyntaxNode OriginatingNode { get; }
        [CanBeNull]
        public ISymbol OriginatingSymbol { get; }
        
        [NotNull]
        public CodeGenerationUnitAndSnapshot CodeGenerationUnitAndSnapshot { get; }

        #endregion

        [NotNull]
        public ITextView TextView { get; }

        [NotNull]
        public ITextBuffer TextBuffer => CodeGenerationUnitAndSnapshot.Snapshot.TextBuffer;

        public EditorSettings GetEditorSettings() {
            return TextView.GetEditorSettings();
        }

        public CodeFixContext GetCodeFixContext() {
            int position   = CaretPoint.Position;
            var context = new CodeFixContext(
                position          : position, 
                codeGenerationUnit: CodeGenerationUnitAndSnapshot.CodeGenerationUnit, 
                editorSettings    : GetEditorSettings());
            return context;
        }
    }
}