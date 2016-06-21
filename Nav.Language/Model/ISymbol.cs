namespace Pharmatechnik.Nav.Language {

    public partial interface ISymbol: IExtent {

        // TODO NotNull?
        string Name { get; }
        Location Location { get; }
        // TODO Diagnostics hier anhängen?
    }
}