namespace Pharmatechnik.Nav.Language {

    sealed class InitTransition : Transition, IInitTransition {
        
        public InitTransition(TransitionDefinitionSyntax syntax, 
            ITaskDefinitionSymbol containingTask, 
            InitNodeReferenceSymbol initNodeReference, 
            EdgeModeSymbol edgeMode, 
            NodeReferenceSymbol targetReference, 
            SymbolCollection<TriggerSymbol> triggers) 
            : base(syntax, containingTask, initNodeReference, edgeMode, targetReference, triggers) {
        }

        public IInitNodeReferenceSymbol InitNodeReference => (IInitNodeReferenceSymbol)SourceReference;
    }
}