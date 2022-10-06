#region Using Directives

using System.Collections.Generic;
using System.Linq;

#endregion

namespace Pharmatechnik.Nav.Language; 

public static class ChoiceNodeSymbolExtensions {

    public static IEnumerable<Call> ExpandCalls(this IChoiceNodeSymbol source) {

        return ExpandOutgoingsImpl<INodeSymbol>(source, new HashSet<ISourceNodeSymbol>()).Distinct(CallComparer.Default);

        static IEnumerable<Call> ExpandOutgoingsImpl<T>(IChoiceNodeSymbol sourceNode, ISet<ISourceNodeSymbol> seenNodes) where T : class, INodeSymbol {

            if (sourceNode == null) {
                yield break;
            }

            if (seenNodes.Contains(sourceNode)) {
                yield break;
            }

            seenNodes.Add(sourceNode);

            foreach (var edge in sourceNode.Outgoings) {
                if (edge.TargetReference?.Declaration is IChoiceNodeSymbol sn) {
                    foreach (var call in ExpandOutgoingsImpl<T>(sn, seenNodes)) {
                        yield return call;
                    }
                } else if (edge.TargetReference?.Declaration is T targetNode &&
                           edge.EdgeMode != null) {
                    // Nur Edges mit einem definiertem Edge Mode ergeben einen Call
                    yield return new Call(targetNode, edge.EdgeMode);
                }
            }
        }
    }

}