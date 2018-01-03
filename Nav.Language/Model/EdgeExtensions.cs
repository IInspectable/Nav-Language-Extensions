#region Using Directives

using System.Linq;
using System.Collections.Generic;

#endregion

namespace Pharmatechnik.Nav.Language {

    public static class EdgeExtensions {

        public static IEnumerable<Call> GetReachableCalls(this IEdge source) {

            return GetReachableCallsImpl<INodeSymbol>(source, new HashSet<IEdge>())
                  .Select(c => new Call(c.Node, c.EdgeMode))
                  .Distinct(Call.EquivalenceComparer);

            IEnumerable<Call> GetReachableCallsImpl<T>(IEdge edge, HashSet<IEdge> seenEdges) where T : class, INodeSymbol {

                if (seenEdges.Contains(edge)) {
                    yield break;
                }

                seenEdges.Add(edge);

                if (!(edge?.TargetReference?.Declaration is T targetNode)) {
                    yield break;
                }

                // Choices auflösen
                if (targetNode is IChoiceNodeSymbol choiceNode) {
                    foreach (var call in choiceNode.Outgoings.SelectMany(e => GetReachableCallsImpl<T>(e, seenEdges))) {
                        yield return call;
                    }
                } else {
                    yield return new Call(targetNode, edge.EdgeMode);
                }
            }
        }

        public static bool IsReachable(this IEdge source) {

            return IsReachableImpl(source, new HashSet<IEdge>());

            bool IsReachableImpl(IEdge edge, HashSet<IEdge> seenEdges) {

                if (seenEdges.Contains(edge)) {
                    return false;
                }

                seenEdges.Add(edge);

                var sourceNode = edge?.SourceReference?.Declaration;
                switch (sourceNode) {
                    case null:
                        return false;
                    case ITargetNode targetNode:
                        return targetNode.Incomings.Any(e => IsReachableImpl(e, seenEdges));
                    case ISourceNode _: // Ein Source Node, der kein Target Node ist, ist immer der Anfang und damit per Definition erreichbar
                        return true;
                }

                return false;
            }
        }

    }

}