using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Pharmatechnik.Nav.Language.Extension.Commands.Extensibility;

namespace Pharmatechnik.Nav.Language.Extension.Commands {

    enum NavigateDirection {
        Up = -1,
        Down = 1,
    }

    class NavigateToHighlightedReferenceCommandArgs : CommandArgs {

        public NavigateToHighlightedReferenceCommandArgs(ITextView textView, ITextBuffer subjectBuffer, NavigateDirection direction)
            : base(textView, subjectBuffer) {

            Direction = direction;
        }

        public NavigateDirection Direction { get; }
    }
}