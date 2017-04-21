#region Using Directives

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Commands {

    class ReturnKeyCommandArgs : CommandArgs {
        public ReturnKeyCommandArgs(ITextView textView, ITextBuffer subjectBuffer)
            : base(textView, subjectBuffer) {
        }
    }
}