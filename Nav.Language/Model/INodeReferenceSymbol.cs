#region Using Directives

using JetBrains.Annotations;

#endregion

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
        IEdge Edge { get; }
    }
}