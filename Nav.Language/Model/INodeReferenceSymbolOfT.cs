using JetBrains.Annotations;

namespace Pharmatechnik.Nav.Language {

    public interface INodeReferenceSymbol<out T> : INodeReferenceSymbol where T : INodeSymbol {
        [CanBeNull]
        new T Declaration { get; }
    }
}