namespace Pharmatechnik.Nav.Language {

    sealed class ChoiceTransition : Transition, IChoiceTransition {

        public ChoiceTransition(TransitionDefinitionSyntax syntax, 
            ITaskDefinitionSymbol containingTask, 
            ChoiceNodeReferenceSymbol choiceReference, 
            EdgeModeSymbol edgeMode, 
            NodeReferenceSymbol targetReference, 
            SymbolCollection<TriggerSymbol> triggers) 
            : base(syntax, containingTask, choiceReference, edgeMode, targetReference, triggers) {
        }

        public IChoiceNodeReferenceSymbol ChoiceNodeReference => (IChoiceNodeReferenceSymbol) SourceReference;
    }
}