using System.Linq;
using System.Collections.Immutable;

namespace Pharmatechnik.Nav.Language.Extension.NavigationBar {

    class MemberNavigationItemBuilder : NavigationItemBuilderBase {

        public static ImmutableList<NavigationItem> Build(CodeGenerationUnit codeGenerationUnit) {
            return BuildCore(codeGenerationUnit, new MemberNavigationItemBuilder());
        }

        public override void VisitTaskDefinitionSymbol(ITaskDefinitionSymbol taskDefinitionSymbol) {
            foreach (var symbol in taskDefinitionSymbol.Transitions.SelectMany(trans=> trans.Symbols())) {
                Visit(symbol);
            }
        }

        public override void VisitSignalTriggerSymbol(ISignalTriggerSymbol signalTriggerSymbol) {
            NavigationItems.Add(new NavigationItem(signalTriggerSymbol.Name, TriggerSymbolImageIndex, signalTriggerSymbol.Location, signalTriggerSymbol.Start));
        }
    }
}