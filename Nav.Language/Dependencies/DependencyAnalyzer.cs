#region Using Directives

using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

#endregion

namespace Pharmatechnik.Nav.Language.Dependencies; 

public static class DependencyAnalyzer {

    public static ImmutableList<Dependency> CollectTasksDefinitionDependencies(IEnumerable<CodeGenerationUnit> codeGenerationUnits) {
        return codeGenerationUnits.SelectMany(CollectTasksDefinitionDependencies).ToImmutableList();
    }

    public static ImmutableList<Dependency> CollectTasksDefinitionDependencies(CodeGenerationUnit codeGenerationUnit) {
        return codeGenerationUnit.TaskDefinitions.SelectMany(CollectTasksDefinitionDependencies).ToImmutableList();
    }

    public static ImmutableList<Dependency> CollectTasksDefinitionDependencies(ITaskDefinitionSymbol taskDefinition) {
        return taskDefinition.NodeDeclarations
                             .OfType<ITaskNodeSymbol>()
                             .Where(tn => tn.Incomings.Any() && tn.Declaration != null)
                             .Select(taskNode => new Dependency(
                                         usingItem: DependencyItem.FromSymbol(taskNode),
                                         usedItem : DependencyItem.FromSymbol(taskNode.Declaration)))
                             .ToImmutableList();         
    }
}

// VersionStamp
// BackgroundParser