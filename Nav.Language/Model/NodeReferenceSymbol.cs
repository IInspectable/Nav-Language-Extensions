using JetBrains.Annotations;

namespace Pharmatechnik.Nav.Language {

    sealed partial class NodeReferenceSymbol : Symbol, INodeReferenceSymbol {
        
        public NodeReferenceSymbol(string name, Location location, INodeSymbol declaration) : base(name, location) {
            Declaration = declaration;
        }

        [CanBeNull]
        public INodeSymbol Declaration { get; }
    }
}