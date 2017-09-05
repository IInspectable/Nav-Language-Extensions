#region Using Directives

using System;

#endregion

namespace Pharmatechnik.Nav.Language.CodeFixes {

    public sealed class EditorSettings {
        
        public EditorSettings(int tabSize, string newLine) {
            if (tabSize < 0) {
                throw new ArgumentOutOfRangeException();
            }
            TabSize = tabSize;
            NewLine = newLine ?? throw new ArgumentNullException(nameof(newLine));
        }

        public int TabSize { get; }
        public string NewLine { get; }
    }
}