#region Using Directives

using System.Collections.Generic;

#endregion

namespace Pharmatechnik.Nav.Language {
    public static class NodeSymbolExtensions {

        public static IEnumerable<Call> GetOutgoingCalls(this INodeSymbol nodeSymbol) {
            
            foreach (var edge in nodeSymbol.GetOutgoingEdges()) {

                var node     = edge.Target?.Declaration;
                var edgeMode = edge.EdgeMode;

                if (node == null || edge.EdgeMode == null || node.Name == null) {
                    continue;
                }

                yield return new Call(node, edgeMode);
            }
        }

        // TODO Ist das nicht per Semantic Model Check sichergestellt?
        public static IEnumerable<Call> GetDistinctOutgoingCalls(this INodeSymbol nodeSymbol) {

            var nodes = new Dictionary<string, Call>();

            foreach (var call in nodeSymbol.GetOutgoingCalls()) {
                nodes[call.Node.Name] = call;
            }
            return nodes.Values;
        }
    }
}