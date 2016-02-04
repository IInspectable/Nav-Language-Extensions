using JetBrains.Annotations;

namespace Pharmatechnik.Nav.Language {

    public interface INodeReferenceSymbol: ISymbol {
        [CanBeNull]
        INodeSymbol Declaration { get;}
    }
}