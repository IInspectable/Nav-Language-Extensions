using System.Collections.Generic;
using JetBrains.Annotations;

namespace Pharmatechnik.Nav.Language {

    public interface IEdge {

        [CanBeNull]
        INodeReferenceSymbol Source { get; }

        [CanBeNull]
        IEdgeModeSymbol EdgeMode { get; }

        [CanBeNull]
        INodeReferenceSymbol Target { get; }

        IEnumerable<ISymbol> Symbols();   
    }
}