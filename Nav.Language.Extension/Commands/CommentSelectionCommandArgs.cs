#region Using Directives

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Commands {

    class CommentSelectionCommandArgs : CommandArgs {
        public CommentSelectionCommandArgs(IWpfTextView textView, ITextBuffer subjectBuffer)
            : base(textView, subjectBuffer) {
        }
    }
}