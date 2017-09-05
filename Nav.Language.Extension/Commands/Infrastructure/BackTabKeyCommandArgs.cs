#region Using Directives

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Commands {

    class BackTabKeyCommandArgs : CommandArgs {
        public BackTabKeyCommandArgs(IWpfTextView textView, ITextBuffer subjectBuffer)
            : base(textView, subjectBuffer) {
        }
    }
}