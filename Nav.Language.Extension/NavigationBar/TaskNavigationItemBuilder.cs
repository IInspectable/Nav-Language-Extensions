using System.Collections.Immutable;

namespace Pharmatechnik.Nav.Language.Extension.NavigationBar {

    class TaskNavigationItemBuilder : NavigationItemBuilderBase {
        
        public static ImmutableList<NavigationItem> Build(CodeGenerationUnit codeGenerationUnit) {
            return BuildCore(codeGenerationUnit, new TaskNavigationItemBuilder());
        }

        public override void VisitTaskDefinitionSymbol(ITaskDefinitionSymbol taskDefinitionSymbol) {
            NavigationItems.Add(new NavigationItem(taskDefinitionSymbol.Name, TaskDefinitionImageIndex, taskDefinitionSymbol.Location));
        }

        public override void VisitTaskDeclarationSymbol(ITaskDeclarationSymbol taskDeclarationSymbol) {
            NavigationItems.Add(new NavigationItem(taskDeclarationSymbol.Name, TaskDeclarationImageIndex, taskDeclarationSymbol.Location));
        }
    }
}