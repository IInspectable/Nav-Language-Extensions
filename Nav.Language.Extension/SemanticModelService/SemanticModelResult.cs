#region Using Directives

using JetBrains.Annotations;
using Microsoft.VisualStudio.Text;

#endregion

namespace Pharmatechnik.Nav.Language.Extension {

    sealed class SemanticModelResult {

        internal SemanticModelResult([NotNull] CompilationUnit compilationUnit, [NotNull] ITextSnapshot snapshot) {
            CompilationUnit = compilationUnit;
            Snapshot = snapshot;
        }

        [NotNull]
        public CompilationUnit CompilationUnit { get; }

        [NotNull]
        public ITextSnapshot Snapshot { get; }

        public bool IsCurrent(SnapshotSpan snapshotSpan) {
            return Snapshot.Version.VersionNumber == snapshotSpan.Snapshot.Version.VersionNumber;
        }

        public bool IsCurrent(ITextSnapshot snapshot) {
            return Snapshot.Version.VersionNumber == snapshot.Version.VersionNumber;
        }
    }

}