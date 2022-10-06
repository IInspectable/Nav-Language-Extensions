#region Using Directives

using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language; 

public interface ISemanticModelProviderFactory {

    [NotNull] 
    ISemanticModelProvider CreateProvider(ISyntaxProvider syntaxProvider);

}