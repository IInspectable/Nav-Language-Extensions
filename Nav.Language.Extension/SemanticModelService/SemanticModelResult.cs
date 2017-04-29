#region Using Directives

using JetBrains.Annotations;
using Microsoft.VisualStudio.Text;

#endregion

namespace Pharmatechnik.Nav.Language.Extension {

    sealed class SemanticModelResult {

        internal SemanticModelResult([NotNull] CodeGenerationUnit codeGenerationUnit, [NotNull] ITextSnapshot snapshot) {
            CodeGenerationUnit = codeGenerationUnit;
            Snapshot = snapshot;
        }

        [NotNull]
        public CodeGenerationUnit CodeGenerationUnit { get; }

        [NotNull]
        public ITextSnapshot Snapshot { get; }

        public bool IsCurrent(SnapshotSpan snapshotSpan) {
            return Snapshot.Version.VersionNumber == snapshotSpan.Snapshot.Version.VersionNumber;
        }

        public bool IsCurrent(ITextSnapshot snapshot) {
            // TODO ReiteratedVersionNumber verwenden? => Undo würde "optimiert"
            if(snapshot != Snapshot) {
                return false;
            }
            return Snapshot.Version.VersionNumber == snapshot.Version.VersionNumber;
        }

        public bool IsCurrent(ITextBuffer textBuffer) {
            return IsCurrent(textBuffer.CurrentSnapshot);
        }        
    }
}