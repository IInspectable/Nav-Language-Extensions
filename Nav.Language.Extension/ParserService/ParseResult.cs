using Microsoft.VisualStudio.Text;

namespace Pharmatechnik.Nav.Language.Extension {

    sealed class ParseResult {

        internal ParseResult(SyntaxTree syntaxTree, ITextSnapshot snapshot) {
            SyntaxTree = syntaxTree;
            Snapshot   = snapshot;
        }

        public SyntaxTree SyntaxTree { get; }

        public ITextSnapshot Snapshot { get; }

        public bool IsCurrent(SnapshotSpan snapshotSpan) {
            return Snapshot.Version.VersionNumber == snapshotSpan.Snapshot.Version.VersionNumber;
        }

        public bool IsCurrent(ITextSnapshot snapshot) {
            return Snapshot.Version.VersionNumber == snapshot.Version.VersionNumber;
        }
    }
}
