#region Using Directives

using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen {

    public class PathProviderFactory {

        public static readonly PathProviderFactory Default = new PathProviderFactory();

        [NotNull]
        public virtual IPathProvider CreatePathProvider(ITaskDefinitionSymbol taskDefinition) {
            return new PathProvider(taskDefinition);
        }
    }
}