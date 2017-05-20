#region Using Directives

using System;
using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen {

    public class PathProviderFactory: IPathProviderFactory {

        public static readonly PathProviderFactory Default = new PathProviderFactory();

        [NotNull]
        public virtual IPathProvider CreatePathProvider(ITaskDefinitionSymbol taskDefinition) {
            return new PathProvider(taskDefinition ?? throw new ArgumentNullException(nameof(taskDefinition)));
        }
    }
}