#region Using Directives

using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language {

    public interface IEdgeModeSymbol: ISymbol {

        EdgeMode EdgeMode { get; }

        [NotNull]
        IEdge Edge { get; }

        string DisplayName { get; }

    }

}