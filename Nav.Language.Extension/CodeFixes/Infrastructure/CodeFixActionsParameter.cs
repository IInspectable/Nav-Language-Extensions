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
        
        public CodeFixActionsParameter(SnapshotSpan range, CodeGenerationUnitAndSnapshot codeGenerationUnitAndSnapshot, ITextView textView) {
            TextView = textView ?? throw new ArgumentNullException(nameof(textView));
            CodeGenerationUnitAndSnapshot = codeGenerationUnitAndSnapshot ?? throw new ArgumentNullException(nameof(codeGenerationUnitAndSnapshot));
            // TODO Range checking
            CodeFixContext = new CodeFixContext(
                range          : new TextExtent(range.Start, range.Length),
                codeGenerationUnit: CodeGenerationUnitAndSnapshot.CodeGenerationUnit,
                editorSettings    : TextView.GetEditorSettings());
        }
        
        [NotNull]
        public CodeGenerationUnitAndSnapshot CodeGenerationUnitAndSnapshot { get; }
        
        [NotNull]
        public ITextView TextView { get; }

        [NotNull]
        public ITextBuffer TextBuffer => CodeGenerationUnitAndSnapshot.Snapshot.TextBuffer;

        [NotNull]
        public CodeFixContext CodeFixContext { get; }
    }
}