namespace Pharmatechnik.Nav.Language.CodeGen {

    public interface IPathProviderFactory {
        IPathProvider CreatePathProvider(ITaskDefinitionSymbol taskDefinition);
    }
}