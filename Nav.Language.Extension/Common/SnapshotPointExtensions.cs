#region Using Directives

using Microsoft.VisualStudio.Text;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Common {

    static class SnapshotPointExtensions {

        public static SnapshotSpan ToSnapshotSpan(this SnapshotPoint snapshotPoint) {
            return new SnapshotSpan(snapshotPoint.Snapshot, snapshotPoint.Position, 0);
        }
        
        public static SnapshotSpan? ToSnapshotSpan(this SnapshotPoint? snapshotPoint) {
            return snapshotPoint?.ToSnapshotSpan();
        }
    }
}