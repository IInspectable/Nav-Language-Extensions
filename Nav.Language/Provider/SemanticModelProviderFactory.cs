#region Using Directives

using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language; 

public class SemanticModelProviderFactory: ISemanticModelProviderFactory {

    public static readonly ISemanticModelProviderFactory Default = new SemanticModelProviderFactory();

    [NotNull]
    public ISemanticModelProvider CreateProvider(ISyntaxProvider syntaxProvider) {
        return new SemanticModelProvider(syntaxProvider);
    }

}