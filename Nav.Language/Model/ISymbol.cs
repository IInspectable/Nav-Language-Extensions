#region Using Directives

using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language {

    public partial interface ISymbol: IExtent {

        // TODO NotNull?
        string Name { get; }

        [NotNull]
        Location Location { get; }
        // TODO Diagnostics hier anhängen?
    }
}