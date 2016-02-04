using Microsoft.VisualStudio.Text;

namespace Pharmatechnik.Nav.Language.Extension.Common {
    static class TextSnapshotExtensions {

        public static SnapshotSpan ToSnapshotSpan(this ITextSnapshot snapshot) {
            return new SnapshotSpan(snapshot, 0, snapshot.Length);
        }

        public static SnapshotSpan GetFullSpan(this ITextSnapshot snapshot) {
           return new SnapshotSpan(snapshot, new Span(0, snapshot.Length));
        }
    }
}