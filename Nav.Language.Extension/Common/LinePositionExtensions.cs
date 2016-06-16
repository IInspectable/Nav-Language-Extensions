using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Pharmatechnik.Nav.Language.Extension.Common {
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
            return new TextExtent(span.Start, span.End);
        }
    }
}
