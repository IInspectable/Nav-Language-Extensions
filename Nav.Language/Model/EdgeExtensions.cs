#region Using Directives

using System.Linq;
using System.Collections.Generic;

#endregion

namespace Pharmatechnik.Nav.Language {

    public static class EdgeExtensions {

        public static IEnumerable<Call> GetDistinctReachableCalls(this IEdge edge) {
            return edge.GetReachableCalls().Distinct(Call.EquivalenceComparer);
        }

        // TODO Unterscheiden zwischen ReachableCalls und ReachableImplemented Tasks
        public static IEnumerable<Call> GetReachableCalls(this IEdge edge, HashSet<IEdge> seenEdges = null) {

            seenEdges = seenEdges ?? new HashSet<IEdge>();

            if (seenEdges.Contains(edge)) {
                yield break;
            }

            seenEdges.Add(edge);

            var targetNode = edge?.TargetReference?.Declaration;
            if (targetNode == null) {
                yield break;
            }

            if (targetNode is IChoiceNodeSymbol choiceNode) {
                foreach (var call in choiceNode.Outgoings.SelectMany(e => GetReachableCalls(e, seenEdges))) {
                    yield return call;
                }
            } else if (targetNode is ITaskNodeSymbol taskNode && taskNode.CodeNotImplemented()) {
                // Skip Not Implemented tasks
            } else {
                yield return new Call(targetNode, edge.EdgeMode);
            }
        }

        public static bool IsReachable(this IEdge edge, HashSet<IEdge> seenEdges = null) {
            seenEdges = seenEdges ?? new HashSet<IEdge>();

            if (seenEdges.Contains(edge)) {
                return false;
            }
            
            seenEdges.Add(edge);

            var sourceNode = edge?.SourceReference?.Declaration;
            if (sourceNode == null) {
                return false;
            }

            if (sourceNode is IInitNodeSymbol ) {
                return true;
            }

            if (sourceNode is IGuiNodeSymbol) {
                return true;
            }

            if (sourceNode is IChoiceNodeSymbol choiceNode) {
                return choiceNode.Incomings.Any(e => IsReachable(e, seenEdges));
            }

            if (sourceNode is ITaskNodeSymbol taskNode) {
                return taskNode.Incomings.Any(e => IsReachable(e, seenEdges));
            }

            return false;
        }

    }

}