﻿#region Using Directives

using System;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Commands.Extensibility {

    abstract class CommandArgs {

        /// <summary>
        /// The text buffer of where the caret is when the command happens.
        /// </summary>
        public ITextBuffer SubjectBuffer { get; }

        /// <summary>
        /// The text view that originated this command.
        /// </summary>
        public ITextView TextView { get; }

        protected CommandArgs(ITextView textView, ITextBuffer subjectBuffer) {
            TextView      = textView      ?? throw new ArgumentNullException(nameof(textView));
            SubjectBuffer = subjectBuffer ?? throw new ArgumentNullException(nameof(subjectBuffer));
        }
    }
}