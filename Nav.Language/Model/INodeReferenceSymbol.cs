using JetBrains.Annotations;

namespace Pharmatechnik.Nav.Language {

    public enum NodeReferenceType {
        Source,
        Target
    }

    public interface INodeReferenceSymbol: ISymbol {
        [CanBeNull]
        INodeSymbol Declaration { get;}
        NodeReferenceType Type { get; }
        [NotNull]
        ITransition Transition { get; }
    }
}