namespace Pharmatechnik.Nav.Language {

    public interface ICodeSymbol : ISymbol { }

    public interface ICodeParameterSymbol : ICodeSymbol {
        
        string ParameterName { get; }
        string ParameterType { get; }
    }
}