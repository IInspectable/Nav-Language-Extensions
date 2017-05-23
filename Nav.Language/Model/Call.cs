#region Using Directives

using System;

#endregion

namespace Pharmatechnik.Nav.Language {
    public sealed class Call {
        public Call(INodeSymbol node, IEdgeModeSymbol edgeMode) {
            Node     = node     ?? throw new ArgumentNullException(nameof(node));
            EdgeMode = edgeMode ?? throw new ArgumentNullException(nameof(edgeMode));
        }
        public INodeSymbol Node { get; }
        public IEdgeModeSymbol EdgeMode { get; }
    }
}