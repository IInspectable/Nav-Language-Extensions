#region Using Directives

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Commands {

    class TypeCharCommandArgs : CommandArgs {
        public TypeCharCommandArgs(ITextView textView, ITextBuffer subjectBuffer, char typedChar)
            : base(textView, subjectBuffer) {
            TypedChar = typedChar;
        }

        /// <summary>
        /// The character that was typed.
        /// </summary>
        public char TypedChar { get; }
    }
}