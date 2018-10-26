#region Using Directives

using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language {

    public static class TaskDefinitionSymbolExtensions {

        [CanBeNull]
        public static INodeSymbol TryFindNode(this ITaskDefinitionSymbol taskDefinition, string name) {
            return taskDefinition?.NodeDeclarations.TryFindSymbol(name);
        }

        [CanBeNull]
        public static T TryFindNode<T>(this ITaskDefinitionSymbol taskDefinition, string name) where T : class, INodeSymbol {
            return taskDefinition?.NodeDeclarations.TryFindSymbol(name) as T;
        }

    }

}