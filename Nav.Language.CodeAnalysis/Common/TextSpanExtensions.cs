#region Using Directives

using Microsoft.CodeAnalysis.Text;

using Pharmatechnik.Nav.Language.Text;

#endregion

namespace Pharmatechnik.Nav.Language.CodeAnalysis.Common; 

public static class TextSpanExtensions {

    public static TextExtent ToTextExtent(this TextSpan span) {
        return new(start: span.Start, length: span.Length);
    }

    public static TextSpan Trim(this TextSpan span, TextSpan range) {

        int start = span.Start.Trim(start: range.Start, end: range.End);
        int end   = span.End.Trim(start: range.Start, end: range.End);

        return TextSpan.FromBounds(start, end);

    }

}