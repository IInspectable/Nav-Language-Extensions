#region Using Directives

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Commands {

    class CommitUniqueCompletionListItemCommandArgs : CommandArgs {
        public CommitUniqueCompletionListItemCommandArgs(IWpfTextView textView, ITextBuffer subjectBuffer)
            : base(textView, subjectBuffer) {
        }
    }
}