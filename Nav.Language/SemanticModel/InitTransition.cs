namespace Pharmatechnik.Nav.Language; 

sealed class InitTransition: Transition, IInitTransition {

    public InitTransition(TransitionDefinitionSyntax syntax,
                          ITaskDefinitionSymbol containingTask,
                          InitNodeReferenceSymbol initNodeReference,
                          EdgeModeSymbol edgeMode,
                          NodeReferenceSymbol targetReference,
                          ConcatTransition concatTransition)
        : base(syntax, containingTask, initNodeReference, edgeMode, targetReference, concatTransition) {
    }

    public IInitNodeReferenceSymbol InitNodeSourceReference => (IInitNodeReferenceSymbol) SourceReference;

}