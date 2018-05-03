#region Using Directives

using JetBrains.Annotations;

using Pharmatechnik.Nav.Language.Text;

#endregion

namespace Pharmatechnik.Nav.Language {

    public partial interface ISymbol: IExtent {

        // TODO NotNull?
        string Name { get; }

        [NotNull]
        Location Location { get; }
    }
}