namespace Pharmatechnik.Nav.Language {

    sealed class TriggerTransition : Transition, ITriggerTransition {

        public TriggerTransition(TransitionDefinitionSyntax syntax, 
            ITaskDefinitionSymbol containingTask, 
            GuiNodeReferenceSymbol sourceReference, 
            EdgeModeSymbol edgeMode, 
            NodeReferenceSymbol targetReference, 
            SymbolCollection<TriggerSymbol> triggers) 
            : base(syntax, containingTask, sourceReference, edgeMode, targetReference, triggers) {             
        }

        public IGuiNodeReferenceSymbol GuiNodeReference => (IGuiNodeReferenceSymbol)SourceReference;
        IReadOnlySymbolCollection<ITriggerSymbol> ITriggerTransition.Triggers => Triggers;
    }
}