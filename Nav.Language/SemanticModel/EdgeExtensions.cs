#region Using Directives

using System.Linq;
using System.Collections.Generic;

#endregion

namespace Pharmatechnik.Nav.Language; 

public static class EdgeExtensions {

    public static IEnumerable<Call> GetReachableContinuationCalls(this IEdge source) {

        return source.GetReachableCalls()
                     .Select(call => call.ContinuationCall)
                     .WhereNotNull();
    }

    public static IEnumerable<Call> GetReachableCalls(this IEdge source) {

        return GetReachableCallsImpl<INodeSymbol>(source, new HashSet<IEdge>()).Distinct(CallComparer.Default);

        static IEnumerable<Call> GetReachableCallsImpl<T>(IEdge edge, ISet<IEdge> seenEdges) where T : class, INodeSymbol {

            if (edge == null) {
                yield break;
            }

            if (!seenEdges.Add(edge)) {
                yield break;
            }

            if (edge.TargetReference?.Declaration is not T targetNode) {
                yield break;
            }

            // Choices auflösen
            if (targetNode is IChoiceNodeSymbol choiceNode) {
                foreach (var call in choiceNode.Outgoings.SelectMany(e => GetReachableCallsImpl<T>(e, seenEdges))) {
                    yield return call;
                }
            } else if (edge.EdgeMode != null) {
                // Nur Edges mit einem definiertem Edge Mode ergeben einen Call
                yield return new Call(targetNode, edge);
            }
        }
    }

    public static IEnumerable<IConcatTransition> GetReachableContinuations(this IEdge source) {
        return source.GetReachableEdges()
                     .OfType<IConcatableEdge>()
                     .Select(edge => edge.ConcatTransition)
                     .WhereNotNull();
    }

    public static IEnumerable<IEdge> GetReachableEdges(this IEdge source) {
        return GetReachableEdges<INodeSymbol>(source);
    }

    public static IEnumerable<IEdge> GetReachableEdges<TTargetNode>(this IEdge source) where TTargetNode : class, INodeSymbol {

        return GetReachableEdgesImpl<TTargetNode>(source, new HashSet<IEdge>()).Distinct();

        static IEnumerable<IEdge> GetReachableEdgesImpl<T>(IEdge edge, ISet<IEdge> seenEdges) where T : class, INodeSymbol {

            if (edge == null) {
                yield break;
            }

            if (!seenEdges.Add(edge)) {
                yield break;
            }

            if (edge.TargetReference?.Declaration is not T targetNode) {
                yield break;
            }

            // Choices auflösen
            if (targetNode is IChoiceNodeSymbol choiceNode) {
                foreach (var reachableEdge in choiceNode.Outgoings.SelectMany(e => GetReachableEdgesImpl<T>(e, seenEdges))) {
                    yield return reachableEdge;
                }
            } else if (edge.EdgeMode != null) {
                // Nur Edges mit einem definiertem Edge Mode ergeben einen Call
                yield return edge;
            }
        }
    }

    public static bool IsReachable(this IEdge source) {

        return IsReachableImpl(source, new HashSet<IEdge>());

        static bool IsReachableImpl(IEdge edge, HashSet<IEdge> seenEdges) {

            if (edge == null) {
                return false;
            }

            if (!seenEdges.Add(edge)) {
                return false;
            }

            var sourceNode = edge.SourceReference?.Declaration;
            return sourceNode switch {
                null                         => false,
                ITargetNodeSymbol targetNode => targetNode.Incomings.Any(e => IsReachableImpl(e, seenEdges)),
                // Ein Source Node, der kein Target Node ist, ist immer der Anfang und damit per Definition erreichbar
                ISourceNodeSymbol => true,
                _                 => false,
            };
        }
    }

}