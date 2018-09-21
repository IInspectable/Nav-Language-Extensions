#region Using Directives

using System.Collections.Immutable;

using Pharmatechnik.Nav.Language.Text;

#endregion

namespace Pharmatechnik.Nav.Language.FindReferences {

    class PrefixPartsBuilder: SymbolVisitor<ImmutableArray<ClassifiedText>> {

        private const char ContainsMemberChar = '\u220B';

        public static ImmutableArray<ClassifiedText> Invoke(ISymbol reference) {
            var visitor = new PrefixPartsBuilder();
            return visitor.Visit(reference);
        }

        private static readonly ImmutableArray<ClassifiedText> DefaultPrefix = ImmutableArray<ClassifiedText>.Empty;

        protected override ImmutableArray<ClassifiedText> DefaultVisit(ISymbol symbol) {
            return DefaultPrefix;
        }

        public override ImmutableArray<ClassifiedText> VisitInitConnectionPointSymbol(IInitConnectionPointSymbol initConnectionPointSymbol) {
            return BuildTaskDefinitionMemberPrefix(initConnectionPointSymbol.TaskDeclaration);
        }

        public override ImmutableArray<ClassifiedText> VisitExitConnectionPointSymbol(IExitConnectionPointSymbol exitConnectionPointSymbol) {
            return BuildTaskDefinitionMemberPrefix(exitConnectionPointSymbol.TaskDeclaration);
        }

        public override ImmutableArray<ClassifiedText> VisitEndConnectionPointSymbol(IEndConnectionPointSymbol endConnectionPointSymbol) {
            return BuildTaskDefinitionMemberPrefix(endConnectionPointSymbol.TaskDeclaration);
        }

        public override ImmutableArray<ClassifiedText> VisitEdgeModeSymbol(IEdgeModeSymbol edgeModeSymbol) {
            return BuildTaskDefinitionMemberPrefix(edgeModeSymbol.Edge.ContainingTask);
        }

        public override ImmutableArray<ClassifiedText> VisitInitNodeSymbol(IInitNodeSymbol initNodeSymbol) {
            return BuildTaskDefinitionMemberPrefix(initNodeSymbol.ContainingTask);
        }

        public override ImmutableArray<ClassifiedText> VisitExitNodeSymbol(IExitNodeSymbol exitNodeSymbol) {
            return BuildTaskDefinitionMemberPrefix(exitNodeSymbol.ContainingTask);
        }

        public override ImmutableArray<ClassifiedText> VisitEndNodeSymbol(IEndNodeSymbol endNodeSymbol) {
            return BuildTaskDefinitionMemberPrefix(endNodeSymbol.ContainingTask);
        }

        public override ImmutableArray<ClassifiedText> VisitTaskNodeSymbol(ITaskNodeSymbol taskNodeSymbol) {
            return BuildTaskDefinitionMemberPrefix(taskNodeSymbol.ContainingTask);
        }

        public override ImmutableArray<ClassifiedText> VisitDialogNodeSymbol(IDialogNodeSymbol dialogNodeSymbol) {
            return BuildTaskDefinitionMemberPrefix(dialogNodeSymbol.ContainingTask);
        }

        public override ImmutableArray<ClassifiedText> VisitViewNodeSymbol(IViewNodeSymbol viewNodeSymbol) {
            return BuildTaskDefinitionMemberPrefix(viewNodeSymbol.ContainingTask);
        }

        public override ImmutableArray<ClassifiedText> VisitChoiceNodeSymbol(IChoiceNodeSymbol choiceNodeSymbol) {
            return BuildTaskDefinitionMemberPrefix(choiceNodeSymbol.ContainingTask);
        }

        public override ImmutableArray<ClassifiedText> VisitSignalTriggerSymbol(ISignalTriggerSymbol signalTriggerSymbol) {
            return BuildTaskDefinitionMemberPrefix(signalTriggerSymbol.Transition.ContainingTask);
        }

        public override ImmutableArray<ClassifiedText> VisitSpontaneousTriggerSymbol(ISpontaneousTriggerSymbol spontaneousTriggerSymbol) {
            return BuildTaskDefinitionMemberPrefix(spontaneousTriggerSymbol.Transition.ContainingTask);
        }

        public override ImmutableArray<ClassifiedText> VisitTaskNodeAliasSymbol(ITaskNodeAliasSymbol taskNodeAliasSymbol) {
            return Visit(taskNodeAliasSymbol.TaskNode);
        }

        public override ImmutableArray<ClassifiedText> VisitInitNodeAliasSymbol(IInitNodeAliasSymbol initNodeAliasSymbol) {
            return Visit(initNodeAliasSymbol.InitNode);
        }

        public override ImmutableArray<ClassifiedText> VisitNodeReferenceSymbol(INodeReferenceSymbol nodeReferenceSymbol) {
            return BuildTaskDefinitionMemberPrefix(nodeReferenceSymbol.Edge.ContainingTask);
        }

        public override ImmutableArray<ClassifiedText> VisitExitConnectionPointReferenceSymbol(IExitConnectionPointReferenceSymbol exitConnectionPointReferenceSymbol) {
            return BuildTaskDefinitionMemberPrefix(exitConnectionPointReferenceSymbol.ExitTransition.ContainingTask);
        }

        ImmutableArray<ClassifiedText> BuildTaskDefinitionMemberPrefix(ITaskDeclarationSymbol taskDeclaration) {
            return taskDeclaration != null ? BuildTaskMemberPrefix(taskDeclaration.Name) : DefaultPrefix;
        }

        ImmutableArray<ClassifiedText> BuildTaskDefinitionMemberPrefix(ITaskDefinitionSymbol taskDefinition) {
            return taskDefinition != null ? BuildTaskMemberPrefix(taskDefinition.Name) : DefaultPrefix;
        }

        ImmutableArray<ClassifiedText> BuildTaskMemberPrefix(string containingName) {
            return new[] {

                ClassifiedTexts.TaskName(containingName),
                ClassifiedTexts.Text(ContainsMemberChar)

            }.ToImmutableArray();

        }

    }

}