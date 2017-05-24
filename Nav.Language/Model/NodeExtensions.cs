#region Using Directives

using System.Linq;
using System.Collections.Generic;

#endregion

namespace Pharmatechnik.Nav.Language {
    public static class NodeExtensions {
        public static IEnumerable<Call> GetReachableCalls(this ITaskNodeSymbol node, HashSet<IEdge> seenEdges = null) {
            seenEdges = seenEdges ?? new HashSet<IEdge>();
            return node.Outgoings.SelectMany(edge => edge.GetReachableCalls(seenEdges));
        }
    }
}