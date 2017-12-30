#region Using Directives

using System.Linq;
using System.Collections.Generic;

#endregion

namespace Pharmatechnik.Nav.Language {

    public static class EdgeExtensions {

        public static IEnumerable<Call> GetReachableCalls(this IEdge edge) {
            return edge.GetReachableCallsImpl<INodeSymbol>(onlyImplemented:false)
                       .Select(c => new Call(c.Node, c.EdgeMode))
                       .Distinct(Call.EquivalenceComparer);
        }

        public static IEnumerable<Call> GetReachableImplementedCalls(this IEdge edge)
        {
            return edge.GetReachableCallsImpl<INodeSymbol>(onlyImplemented: true)
                .Select(c => new Call(c.Node, c.EdgeMode))
                .Distinct(Call.EquivalenceComparer);
        }

        static IEnumerable<Call> GetReachableCallsImpl<T>(this IEdge edge, bool onlyImplemented, HashSet<IEdge> seenEdges = null) where T : class, INodeSymbol {

            seenEdges = seenEdges ?? new HashSet<IEdge>();

            if(seenEdges.Contains(edge)) {
                yield break;
            }

            seenEdges.Add(edge);

            if(!(edge?.TargetReference?.Declaration is T targetNode)) {
                yield break;
            }

            if(targetNode is IChoiceNodeSymbol choiceNode) {
                foreach(var call in choiceNode.Outgoings.SelectMany(e => GetReachableCallsImpl<T>(e, onlyImplemented, seenEdges))) {
                    yield return call;
                }
            } else if(onlyImplemented && targetNode is ITaskNodeSymbol taskNode && (taskNode.CodeNotImplemented() || taskNode.CodeDoNotInject())) {
                // Skip Not Implemented tasks
            } else {
                yield return new Call(targetNode, edge.EdgeMode);
            }
        }

        public static bool IsReachable(this IEdge edge, HashSet<IEdge> seenEdges = null) {
            seenEdges = seenEdges ?? new HashSet<IEdge>();

            if(seenEdges.Contains(edge)) {
                return false;
            }

            seenEdges.Add(edge);

            var sourceNode = edge?.SourceReference?.Declaration;
            if(sourceNode == null) {
                return false;
            }

            if(sourceNode is IInitNodeSymbol) {
                return true;
            }

            if(sourceNode is IGuiNodeSymbol) {
                return true;
            }

            if(sourceNode is IChoiceNodeSymbol choiceNode) {
                return choiceNode.Incomings.Any(e => IsReachable(e, seenEdges));
            }

            if(sourceNode is ITaskNodeSymbol taskNode) {
                return taskNode.Incomings.Any(e => IsReachable(e, seenEdges));
            }

            return false;
        }

    }

}