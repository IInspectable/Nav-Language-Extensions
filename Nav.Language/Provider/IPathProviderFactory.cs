namespace Pharmatechnik.Nav.Language {

    public interface IPathProviderFactory {
        IPathProvider CreatePathProvider(ITaskDefinitionSymbol taskDefinition);
    }
}