#region Using Directives

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Commands {

    enum NavigateDirection {
        Up  = -1,
        Down = 1,
    }

    class NavigateToHighlightedReferenceCommandArgs : CommandArgs {

        public NavigateToHighlightedReferenceCommandArgs(IWpfTextView textView, ITextBuffer subjectBuffer, NavigateDirection direction)
            : base(textView, subjectBuffer) {

            Direction = direction;
        }

        public NavigateDirection Direction { get; }
    }
}