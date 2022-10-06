#region Using Directives

using Microsoft.VisualStudio.Text;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Common; 

static class SymbolExtensions {

    public static SnapshotSpan GetSnapshotSpan(this Location location, ITextSnapshot snapshot) {
        return location.ToSnapshotSpan(snapshot);
    }
        
    public static SnapshotSpan GetSnapshotSpan(this ISymbol symbol, ITextSnapshot snapshot) {
        return GetSnapshotSpan(symbol.Location, snapshot);
    }
}