namespace Pharmatechnik.Nav.Language {

    public partial interface ISymbol: IExtent {

        string Name { get; }
        Location Location { get; }
        // TODO Diagnostics hier anhängen?
    }
}