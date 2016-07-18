#region Using Directives

using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.NavigationBar {

    class TaskNavigationItemBuilder : SymbolVisitor {

        protected TaskNavigationItemBuilder() {
            NavigationItems = new List<NavigationItem>();
            MemberItems     = new List<NavigationItem>();
        }

        public List<NavigationItem> NavigationItems { get; }
        public List<NavigationItem> MemberItems { get; }

        public static ImmutableList<NavigationItem> Build(CodeGenerationUnit codeGenerationUnit) {

            var builder = new TaskNavigationItemBuilder();

            foreach (var symbol in codeGenerationUnit.TaskDefinitions) {
                builder.Visit(symbol);
            }

            foreach (var symbol in codeGenerationUnit.TaskDeclarations) {
                builder.Visit(symbol);
            }

            var items = builder.NavigationItems
                               .OrderBy(ni => ni.Location?.Start??-1)
                               .ToImmutableList();

            return items;
        }

        public override void VisitTaskDefinitionSymbol(ITaskDefinitionSymbol taskDefinitionSymbol) {
         
            foreach (var symbol in taskDefinitionSymbol.Transitions.SelectMany(trans => trans.Symbols())) {
                Visit(symbol);
            }

          //  int max = 100;
          //  for(int i = 0; i< max; i++) {
          //      MemberItems.Add(new NavigationItem($"ImageIndex {i}", i, null, -1));
          //  }
          //  
            NavigationItems.Add(new NavigationItem(
                displayName    : taskDefinitionSymbol.Name, 
                imageIndex     : NavigationImages.Index.TaskDefinition, 
                location       : taskDefinitionSymbol.Syntax.GetLocation(), 
                navigationPoint: taskDefinitionSymbol.Location.Start,
                children       : MemberItems.ToImmutableList()));

            MemberItems.Clear();
        }

        public override void VisitTaskDeclarationSymbol(ITaskDeclarationSymbol taskDeclarationSymbol) {

            // Haben wir bereits in Form der Taskdefinition abgefrühstückt
            // => Jede Taskdefinition ist auch eine Deklaration
            if(taskDeclarationSymbol.Origin == TaskDeclarationOrigin.TaskDefinition) {
                return;
            }

            NavigationItems.Add(new NavigationItem(taskDeclarationSymbol.Name, NavigationImages.Index.TaskDeclaration, taskDeclarationSymbol.Syntax?.GetLocation(), taskDeclarationSymbol.Location.Start));
        }

        public override void VisitSignalTriggerSymbol(ISignalTriggerSymbol signalTriggerSymbol) {
            MemberItems.Add(new NavigationItem(signalTriggerSymbol.Name, NavigationImages.Index.TriggerSymbol, signalTriggerSymbol.Transition.Location, signalTriggerSymbol.Start));
        }
    }
}