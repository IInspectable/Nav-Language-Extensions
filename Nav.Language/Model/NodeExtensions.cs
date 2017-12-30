#region Using Directives

using System.Linq;
using System.Collections.Generic;

#endregion

namespace Pharmatechnik.Nav.Language {

    public static class NodeExtensions {

        public static IEnumerable<Call> GetReachableCalls(this ITaskNodeSymbol node) {
            return node.Outgoings.SelectMany(edge => edge.GetReachableCalls());
        }

        public static IEnumerable<Call> GetReachableImplementedCalls(this IInitNodeSymbol node) {
            return node.Outgoings.SelectMany(edge => edge.GetReachableImplementedCalls());
        }

        public static IEnumerable<Call> GetReachableImplementedCalls(this ITaskNodeSymbol node) {
            return node.Outgoings.SelectMany(edge => edge.GetReachableImplementedCalls());
        }

    }

}