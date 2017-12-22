#region Using Directives

using System;
using System.Collections.Generic;

using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language {

    abstract class Transition : ITransition {

        internal Transition(TransitionDefinitionSyntax syntax, 
                            ITaskDefinitionSymbol containingTask, 
                            NodeReferenceSymbol sourceReference, 
                            EdgeModeSymbol edgeMode, 
                            NodeReferenceSymbol targetReference, 
                            SymbolCollection<TriggerSymbol> triggers)  {

            ContainingTask  = containingTask ?? throw new ArgumentNullException(nameof(containingTask));
            Syntax          = syntax         ?? throw new ArgumentNullException(nameof(syntax));
            SourceReference = sourceReference;
            EdgeMode        = edgeMode;
            TargetReference = targetReference;
            Triggers        = triggers ?? new SymbolCollection<TriggerSymbol>();

            if (sourceReference != null) {                
                sourceReference.Edge   = this;
            }
            if (edgeMode != null) {
                edgeMode.Edge = this;
            }
            if (targetReference != null) {
                targetReference.Edge = this;
            }
            foreach (var trigger in Triggers) {
                trigger.Transition = this;
            }            
        }

        [NotNull]
        public ITaskDefinitionSymbol ContainingTask { get; }

        [NotNull]
        public Location Location => Syntax.GetLocation();

        [NotNull]
        public TransitionDefinitionSyntax Syntax { get; }
        
        [CanBeNull]
        public INodeReferenceSymbol SourceReference { get; }

        [CanBeNull]
        public IEdgeModeSymbol EdgeMode { get; }

        [CanBeNull]
        public INodeReferenceSymbol TargetReference { get; }
        
        [NotNull]
        public SymbolCollection<TriggerSymbol> Triggers { get; }

       

        [NotNull]
        public IEnumerable<ISymbol> Symbols() {

            if(SourceReference != null) {
                yield return SourceReference;
            }

            if (EdgeMode != null) {
                yield return EdgeMode;
            }

            if (TargetReference != null) {
                yield return TargetReference;
            }

            foreach(var trigger in Triggers) {
                yield return trigger;
            }
        }
    }
}