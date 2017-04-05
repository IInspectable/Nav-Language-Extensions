#region Using Directives

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Pharmatechnik.Nav.Language.Extension.Commands.Extensibility;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Commands {

    class CommentSelectionCommandArgs : CommandArgs {
        public CommentSelectionCommandArgs(ITextView textView, ITextBuffer subjectBuffer)
            : base(textView, subjectBuffer) {
        }
    }
}