using System;

namespace Pharmatechnik.Nav.Language {
    [Serializable]
    public abstract class ConnectionPointNodeSyntax : NodeDeclarationSyntax {

        protected ConnectionPointNodeSyntax(TextExtent extent) : base(extent) {
        }        
    }
}