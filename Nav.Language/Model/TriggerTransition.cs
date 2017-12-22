namespace Pharmatechnik.Nav.Language {

    sealed class TriggerTransition : Transition, ITriggerTransition {

        public TriggerTransition(TransitionDefinitionSyntax syntax, 
            ITaskDefinitionSymbol containingTask, 
            NodeReferenceSymbol sourceReference, 
            EdgeModeSymbol edgeMode, 
            NodeReferenceSymbol targetReference, 
            SymbolCollection<TriggerSymbol> triggers) 
            : base(syntax, containingTask, sourceReference, edgeMode, targetReference, triggers) {
        }

        IReadOnlySymbolCollection<ITriggerSymbol> ITriggerTransition.Triggers => Triggers;
    }
}