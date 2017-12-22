namespace Pharmatechnik.Nav.Language {

    sealed class InitNodeReferenceSymbol : NodeReferenceSymbol<IInitNodeSymbol>, IInitNodeReferenceSymbol {

        public InitNodeReferenceSymbol(string name, Location location, IInitNodeSymbol declaration, NodeReferenceType nodeReferenceType)
            : base(name, location, declaration, nodeReferenceType) {
        }
    }
}