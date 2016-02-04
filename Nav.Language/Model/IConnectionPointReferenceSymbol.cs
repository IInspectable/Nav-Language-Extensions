using JetBrains.Annotations;

namespace Pharmatechnik.Nav.Language {

    public interface IConnectionPointReferenceSymbol : ISymbol {
        [CanBeNull]
        IConnectionPointSymbol Declaration { get; }
    }
}