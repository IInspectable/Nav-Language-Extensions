namespace Pharmatechnik.Nav.Language {

    public interface ICodeSymbol : ISymbol { }

    public interface ICodeParameterSymbol : ICodeSymbol {
        
        string ParamterName { get; }
        string ParamterType { get; }
    }
}