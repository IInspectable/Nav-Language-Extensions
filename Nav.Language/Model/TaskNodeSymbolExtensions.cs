#region Using Directives

using System.Linq;
using System.Collections.Generic;

#endregion

namespace Pharmatechnik.Nav.Language {
    public static class TaskNodeSymbolExtensions {

        public static IEnumerable<IConnectionPointSymbol> GetMissingExitTransitionConnectionPoints(this ITaskNodeSymbol taskNode) {

            if (taskNode?.Declaration == null) {
                yield break;
            }

            var expectedExitConnectionPoints = taskNode.Declaration.Exits();
            var actualExitConnectionPoints = taskNode.Outgoings
                                                     .Select(et => et.ConnectionPoint)
                                                     .Where(cp => cp != null)
                                                     .ToList();

            foreach (var expectedExit in expectedExitConnectionPoints) {
                if (!actualExitConnectionPoints.Exists(connectionPointReference => connectionPointReference.Declaration == expectedExit)) {
                    yield return expectedExit;
                }
            }
        }
    }
}
