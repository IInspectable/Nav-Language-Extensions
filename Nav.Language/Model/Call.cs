#region Using Directives

using System;
using System.Collections.Generic;

#endregion

namespace Pharmatechnik.Nav.Language {
    public sealed class Call {

        public Call(INodeSymbol node, IEdgeModeSymbol edgeMode) {
            Node     = node     ?? throw new ArgumentNullException(nameof(node));
            EdgeMode = edgeMode ?? throw new ArgumentNullException(nameof(edgeMode));
        }
        
        public INodeSymbol Node { get; }
        public IEdgeModeSymbol EdgeMode { get; }

        public static readonly IEqualityComparer<Call> EquivalenceComparer = new EquivalenceComparerImpl();

        sealed class EquivalenceComparerImpl : IEqualityComparer<Call>{
            
            public bool Equals(Call x, Call y) {

                if (x == null && y == null) {
                    return true;
                }

                if (x == null | y == null) {
                    return false;
                }

                return x.Node.Name      == y.Node.Name && 
                       x.EdgeMode?.Name == y.EdgeMode?.Name;                             
            }

            public int GetHashCode(Call call) {
                unchecked {
                    return (call.Node.Name.GetHashCode() * 397) ^ (call.EdgeMode?.Name?.GetHashCode() ?? 0);
                }
            }
        }
    }
}