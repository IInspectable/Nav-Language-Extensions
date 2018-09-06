#region Using Directives

using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Threading;

using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion;
using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion.Data;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Completion3 {

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

        public IEnumerable<char> PotentialCommitCharacters => _commitChars;

        public bool ShouldCommitCompletion(char typedChar, SnapshotPoint location, CancellationToken token) {
            // This method is called only when typedChar is among PotentialCommitCharacters
            // in this simple example, all PotentialCommitCharacters do commit, so we always return true.
            return true;
        }

        public CommitResult TryCommit(ITextView view, ITextBuffer buffer, CompletionItem item, ITrackingSpan applicableToSpan, char typedChar, CancellationToken token) {

            if (item.Properties.TryGetProperty<ITrackingSpan>(AsyncCompletionSource.ReplacementTrackingSpanProperty, out var replacementSpan)) {

                using (var edit = buffer.CreateEdit()) {

                    edit.Replace(replacementSpan.GetSpan(buffer.CurrentSnapshot), item.InsertText);
                    edit.Apply();

                    return CommitResult.Handled;
                }
            }

            return CommitResult.Unhandled; // use default commit mechanism.

        }

    }

}