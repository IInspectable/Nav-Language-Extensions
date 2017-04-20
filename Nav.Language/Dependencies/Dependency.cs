#region Using Directives

using System.Linq;

#endregion

using System.Collections.Generic;

namespace Pharmatechnik.Nav.Language.Dependencies {

    public sealed class Dependency {
        
        public Dependency(DependencyItem usingItem, DependencyItem usedItem) {
            UsingItem = usingItem;
            UsedItem  = usedItem;
        }

        public DependencyItem UsingItem { get; }
        public DependencyItem UsedItem { get; }

        public static IEnumerable<Dependency> FromTaskDefinition(ITaskDefinitionSymbol taskDefinition) {

            foreach (var taskNode in taskDefinition.NodeDeclarations
                                                   .OfType<ITaskNodeSymbol>()
                                                   .Where(tn=> tn.Incomings.Any() && tn.Declaration!=null)) {
                
                yield return new Dependency(usingItem: DependencyItem.FromSymbol(taskNode), 
                                            usedItem : DependencyItem.FromSymbol(taskNode.Declaration));
                
            }
        }
    }
}