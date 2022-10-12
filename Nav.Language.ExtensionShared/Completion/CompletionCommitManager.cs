#region Using Directives

using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Immutable;

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion;
using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion.Data;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Completion; 

class CompletionCommitManager: IAsyncCompletionCommitManager {

    // TODO PotentialCommitCharacters aud Sinnhaftigkeit prüfen
    readonly ImmutableArray<char> _commitChars = new[] {
        ' ',
        '\'',
        '"',
        //'.',
        ',',
        ';',
        SyntaxFacts.Colon,
        Path.DirectorySeparatorChar,
        Path.AltDirectorySeparatorChar,
        SyntaxFacts.OpenBracket,
        SyntaxFacts.CloseBracket
    }.ToImmutableArray();

    public bool ShouldCommitCompletion(IAsyncCompletionSession session, SnapshotPoint location, char typedChar, CancellationToken token) {
        return true;
    }

    public CommitResult TryCommit(IAsyncCompletionSession session, ITextBuffer buffer, CompletionItem item, char typedChar, CancellationToken token) {
        if (item.Properties.TryGetProperty<ITrackingSpan>(AsyncCompletionSource.ReplacementTrackingSpanProperty, out var replacementSpan)) {

            using var edit = buffer.CreateEdit();

            edit.Replace(replacementSpan.GetSpan(buffer.CurrentSnapshot), item.InsertText);
            edit.Apply();

            return CommitResult.Handled;

        }

        return CommitResult.Unhandled; // use default commit mechanism.
    }

    public IEnumerable<char> PotentialCommitCharacters => _commitChars;

}