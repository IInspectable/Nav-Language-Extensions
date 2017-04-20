#region Using Directives

using System.Linq;
using System.Collections.Generic;

#endregion

namespace Pharmatechnik.Nav.Language.Dependencies {
    public static class DependencyChecker {

        public static IEnumerable<Dependency> CollectTasksDefinitionDependencies(IEnumerable<CodeGenerationUnit> codeGenerationUnits) {
            return codeGenerationUnits.SelectMany(CollectTasksDefinitionDependencies);
        }

        public static IEnumerable<Dependency> CollectTasksDefinitionDependencies(CodeGenerationUnit codeGenerationUnit) {
            return codeGenerationUnit.TaskDefinitions.SelectMany(CollectTasksDefinitionDependencies);
        }

        public static IEnumerable<Dependency> CollectTasksDefinitionDependencies(ITaskDefinitionSymbol taskDefinition) {

            foreach (var taskNode in taskDefinition.NodeDeclarations
                                                   .OfType<ITaskNodeSymbol>()
                                                   .Where(tn => tn.Incomings.Any() && tn.Declaration != null)) {

                yield return new Dependency(usingItem: DependencyItem.FromSymbol(taskNode),
                                            usedItem : DependencyItem.FromSymbol(taskNode.Declaration));

            }
        }
    }
}