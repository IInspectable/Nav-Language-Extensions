#region Using Directives

using System.Collections.Generic;

#endregion

namespace Pharmatechnik.Nav.Language {
    public static class NodeSymbolExtensions {

        public static IEnumerable<Call> GetOutgoingCalls(this INodeSymbol nodeSynbol) {
            
            foreach (var edge in nodeSynbol.GetOutgoingEdges()) {

                var node     = edge.Target?.Declaration;
                var edgeMode = edge.EdgeMode;

                if (node == null || edge.EdgeMode == null || node.Name == null) {
                    continue;
                }

                yield return new Call(node, edgeMode);
            }
        }

        public static IEnumerable<Call> GetDistinctOutgoingCalls(this INodeSymbol nodeSynbol) {

            var nodes = new Dictionary<string, Call>();

            foreach (var call in nodeSynbol.GetOutgoingCalls()) {
                nodes[call.Node.Name] = call;
            }
            return nodes.Values;
        }
    }
}