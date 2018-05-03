#region Using Directives

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

using Pharmatechnik.Nav.Language.Text;

using LinePosition = Pharmatechnik.Nav.Language.Text.LinePosition;

#endregion

namespace Pharmatechnik.Nav.Language.CodeAnalysis.Common {
    public static class LinePositionExtensions {

        public static LinePosition ToLinePosition(this Microsoft.CodeAnalysis.Text.LinePosition linePosition) {
            return new LinePosition(linePosition.Line, linePosition.Character);            
        }

        public static LinePositionExtent ToLinePositionExtent(this FileLinePositionSpan fileLinePositionSpan) {
            return new LinePositionExtent(
                fileLinePositionSpan.StartLinePosition.ToLinePosition(), 
                fileLinePositionSpan.EndLinePosition.ToLinePosition());
        }
    }

    public static class TextSpanExtensions {

        public static TextExtent ToTextExtent(this TextSpan span) {
            return new TextExtent(start: span.Start, length: span.Length);
        }
    }
}
