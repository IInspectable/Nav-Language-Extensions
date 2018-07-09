#region Using Directives

using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;
using Pharmatechnik.Nav.Language.Extension.Images;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.NavigationBar {

    class NavigationBarTaskItemBuilder : SymbolVisitor {

        protected NavigationBarTaskItemBuilder() {
            NavigationItems = new List<NavigationBarItem>();
            MemberItems     = new List<NavigationBarItem>();
        }

        public List<NavigationBarItem> NavigationItems { get; }
        public List<NavigationBarItem> MemberItems { get; }

        public static ImmutableList<NavigationBarItem> Build(CodeGenerationUnitAndSnapshot codeGenerationUnitAndSnapshot) {

            var codeGenerationUnit = codeGenerationUnitAndSnapshot?.CodeGenerationUnit;
            if(codeGenerationUnit == null) {
                return ImmutableList<NavigationBarItem>.Empty;
            }

            var builder = new NavigationBarTaskItemBuilder();

            foreach (var symbol in codeGenerationUnit.TaskDefinitions) {
                builder.Visit(symbol);
            }

            foreach (var symbol in codeGenerationUnit.TaskDeclarations) {
                builder.Visit(symbol);
            }

            var items = builder.NavigationItems
                               .OrderBy(ni => ni.Start)
                               .ToImmutableList();

            return items;
        }

        public override void VisitTaskDefinitionSymbol(ITaskDefinitionSymbol taskDefinitionSymbol) {
            #if ShowMemberCombobox
            foreach (var symbol in taskDefinitionSymbol.Transitions.SelectMany(trans => trans.Symbols())) {
                Visit(symbol);
            }
            #endif

            NavigationItems.Add(new NavigationBarItem(
                displayName    : taskDefinitionSymbol.Name, 
                imageIndex     : NavigationBarImages.Index.TaskDefinition, 
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

            NavigationItems.Add(new NavigationBarItem(
                displayName    : taskDeclarationSymbol.Name, 
                imageIndex     : NavigationBarImages.Index.TaskDeclaration, 
                location       : taskDeclarationSymbol.Syntax?.GetLocation(), 
                navigationPoint: taskDeclarationSymbol.Location.Start));
        }

        #if ShowMemberCombobox
        public override void VisitSignalTriggerSymbol(ISignalTriggerSymbol signalTriggerSymbol) {
            MemberItems.Add(new NavigationItem(signalTriggerSymbol.Name, NavigationBarImages.Index.TriggerSymbol, signalTriggerSymbol.Transition.Location, signalTriggerSymbol.Start));
        }
        #endif
    }
}