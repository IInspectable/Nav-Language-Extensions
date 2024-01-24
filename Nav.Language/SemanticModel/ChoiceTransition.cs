namespace Pharmatechnik.Nav.Language; 

sealed class ChoiceTransition: Transition, IChoiceTransition {

    public ChoiceTransition(TransitionDefinitionSyntax syntax,
                            ITaskDefinitionSymbol containingTask,
                            ChoiceNodeReferenceSymbol choiceReference,
                            EdgeModeSymbol edgeMode,
                            NodeReferenceSymbol targetReference,
                            ConcatTransition concatTransition)
        : base(syntax, containingTask, choiceReference, edgeMode, targetReference, concatTransition) {
    }

    public IChoiceNodeReferenceSymbol ChoiceNodeSourceReference => (IChoiceNodeReferenceSymbol) SourceReference;

}