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
    
    sealed class InitNodeReferenceSymbol : NodeReferenceSymbol<IInitNodeSymbol>, IInitNodeReferenceSymbol {

        public InitNodeReferenceSymbol(string name, Location location, IInitNodeSymbol declaration, NodeReferenceType nodeReferenceType)
            : base(name, location, declaration, nodeReferenceType) {
        }
    }

    sealed class ChoiceNodeReferenceSymbol : NodeReferenceSymbol<IChoiceNodeSymbol>, IChoiceNodeReferenceSymbol {

        public ChoiceNodeReferenceSymbol(string name, Location location, IChoiceNodeSymbol declaration, NodeReferenceType nodeReferenceType)
            : base(name, location, declaration, nodeReferenceType) {
        }
    }

    sealed class GuiNodeReferenceSymbol : NodeReferenceSymbol<IGuiNodeSymbol>, IGuiNodeReferenceSymbol {

        public GuiNodeReferenceSymbol(string name, Location location, IGuiNodeSymbol declaration, NodeReferenceType nodeReferenceType)
            : base(name, location, declaration, nodeReferenceType) {
        }
    }

    sealed class TaskNodeReferenceSymbol : NodeReferenceSymbol<ITaskNodeSymbol>, ITaskNodeReferenceSymbol {

        public TaskNodeReferenceSymbol(string name, Location location, ITaskNodeSymbol declaration, NodeReferenceType nodeReferenceType)
            : base(name, location, declaration, nodeReferenceType) {
        }
    }
}