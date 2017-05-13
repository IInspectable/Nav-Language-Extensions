using JetBrains.Annotations;

namespace Pharmatechnik.Nav.Language {

    sealed partial class NodeReferenceSymbol : Symbol, INodeReferenceSymbol {

        // ReSharper disable once NotNullMemberIsNotInitialized Transition wird im Ctor der Transition während der Initialisierung gesetzt 
        // In der "freien" Wildbahn" darf hingegen der Null Fall nicht auftreten
        public NodeReferenceSymbol(string name, Location location, INodeSymbol declaration, NodeReferenceType type) : base(name, location) {
            Type        = type;
            Declaration = declaration;
        }

        [CanBeNull]
        public INodeSymbol Declaration { get; }
        public NodeReferenceType Type { get; }
        public IEdge Edge { get; set; }
    }
}