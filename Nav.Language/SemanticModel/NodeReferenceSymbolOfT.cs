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
    
    sealed partial class InitNodeReferenceSymbol : NodeReferenceSymbol<IInitNodeSymbol>, IInitNodeReferenceSymbol {

        public InitNodeReferenceSymbol(string name, Location location, IInitNodeSymbol declaration, NodeReferenceType nodeReferenceType)
            : base(name, location, declaration, nodeReferenceType) {
        }
    }

    sealed partial class ChoiceNodeReferenceSymbol : NodeReferenceSymbol<IChoiceNodeSymbol>, IChoiceNodeReferenceSymbol {

        public ChoiceNodeReferenceSymbol(string name, Location location, IChoiceNodeSymbol declaration, NodeReferenceType nodeReferenceType)
            : base(name, location, declaration, nodeReferenceType) {
        }
    }

    sealed partial class GuiNodeReferenceSymbol : NodeReferenceSymbol<IGuiNodeSymbol>, IGuiNodeReferenceSymbol {

        public GuiNodeReferenceSymbol(string name, Location location, IGuiNodeSymbol declaration, NodeReferenceType nodeReferenceType)
            : base(name, location, declaration, nodeReferenceType) {
        }
    }

    sealed partial class TaskNodeReferenceSymbol : NodeReferenceSymbol<ITaskNodeSymbol>, ITaskNodeReferenceSymbol {

        public TaskNodeReferenceSymbol(string name, Location location, ITaskNodeSymbol declaration, NodeReferenceType nodeReferenceType)
            : base(name, location, declaration, nodeReferenceType) {
        }
    }

    sealed partial class ExitNodeReferenceSymbol : NodeReferenceSymbol<IExitNodeSymbol>, IExitNodeReferenceSymbol {

        public ExitNodeReferenceSymbol(string name, Location location, IExitNodeSymbol declaration, NodeReferenceType nodeReferenceType)
            : base(name, location, declaration, nodeReferenceType) {
        }
    }

    sealed partial class EndNodeReferenceSymbol : NodeReferenceSymbol<IEndNodeSymbol>, IEndNodeReferenceSymbol {

        public EndNodeReferenceSymbol(string name, Location location, IEndNodeSymbol declaration, NodeReferenceType nodeReferenceType)
            : base(name, location, declaration, nodeReferenceType) {
        }
    }
}