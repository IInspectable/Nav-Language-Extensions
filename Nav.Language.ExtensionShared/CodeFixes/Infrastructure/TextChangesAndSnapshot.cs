#region Using Directives

using System.Collections.Generic;
using System.Collections.Immutable;

using Microsoft.VisualStudio.Text;

using Pharmatechnik.Nav.Language.Text;
using Pharmatechnik.Nav.Language.Extension.Common;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.CodeFixes {

    sealed class TextChangesAndSnapshot: AndSnapshot {
        
        public TextChangesAndSnapshot(IEnumerable<TextChange> textChanges, ITextSnapshot snapshot): base(snapshot) {
            TextChanges = textChanges.ToImmutableList();
        }

        public ImmutableList<TextChange> TextChanges { get; }
    }
}