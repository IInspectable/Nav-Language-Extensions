#region Using Directives

using Microsoft.VisualStudio.Text;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Common {
    static class LocationExtensions {

        public static SnapshotSpan ToSnapshotSpan(this Location location, ITextSnapshot textSnapshot) {
            // TODO Adaption von Start und Legth
            return new SnapshotSpan(textSnapshot, start: location.Start, length: location.Length);
        }
    }
}