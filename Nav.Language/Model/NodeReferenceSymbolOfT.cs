using JetBrains.Annotations;

namespace Pharmatechnik.Nav.Language {

    abstract class NodeReferenceSymbol<T> : NodeReferenceSymbol, INodeReferenceSymbol<T> where T: INodeSymbol {

        protected  NodeReferenceSymbol(string name, Location location, T declaration, NodeReferenceType nodeReferenceType)
            : base(name, location, declaration, nodeReferenceType) {
            Declaration = declaration;
        }

        [CanBeNull]
        public new T Declaration { get; }
    }
}