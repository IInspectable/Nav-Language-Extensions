#region Using Directives

using Microsoft.VisualStudio.Text;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Common {

    static class SnapshotPointExtensions {

        public static SnapshotSpan ToSnapshotSpan(this SnapshotPoint snapshotPoint, int length=0) {
            var start = snapshotPoint.Position;
            if (length > 0 && snapshotPoint.Snapshot.Length>=length) {
                var offfset = start + length  - snapshotPoint.Snapshot.Length;
                if (offfset>0) {
                    start -= offfset;
                }
            }
            return new SnapshotSpan(snapshotPoint.Snapshot, start, length);
        }

        public static SnapshotSpan ExtendToLength1(this SnapshotPoint snapshotPoint) {
            var start = snapshotPoint.Position;
            if(snapshotPoint.Snapshot.Length >= 1) {
                var offset = start + 1 - snapshotPoint.Snapshot.Length;
                if(offset > 0) {
                    start -= offset;
                }
                
            }
            return new SnapshotSpan(snapshotPoint.Snapshot, start, 1);
        }

        public static SnapshotSpan? ToSnapshotSpan(this SnapshotPoint? snapshotPoint) {
            return snapshotPoint?.ToSnapshotSpan();
        }
    }
}