#region Using Directives

using Microsoft.CodeAnalysis;

using Pharmatechnik.Nav.Language.Text;

using LinePosition = Pharmatechnik.Nav.Language.Text.LinePosition;

#endregion

namespace Pharmatechnik.Nav.Language.CodeAnalysis.Common {

    public static class LinePositionExtensions {

        public static LinePosition ToLinePosition(this Microsoft.CodeAnalysis.Text.LinePosition linePosition) {
            return new(linePosition.Line, linePosition.Character);
        }

        public static LineRange ToLineRange(this FileLinePositionSpan fileLinePositionSpan) {
            return new(
                fileLinePositionSpan.StartLinePosition.ToLinePosition(),
                fileLinePositionSpan.EndLinePosition.ToLinePosition());
        }

    }

}