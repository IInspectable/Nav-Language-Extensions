using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Pharmatechnik.Nav.Language {

    sealed class Transition : ITransition {

        internal Transition(ITaskDefinitionSymbol taskDefinition, 
                            TransitionDefinitionSyntax syntax, 
                            NodeReferenceSymbol source, 
                            EdgeModeSymbol edgeMode, 
                            NodeReferenceSymbol target, 
                            SymbolCollection<TriggerSymbol> triggers)  {

            if (taskDefinition == null) {
                throw new ArgumentNullException(nameof(taskDefinition));
            }

            if (syntax == null) {
                throw new ArgumentNullException(nameof(syntax));
            }

            TaskDefinition = taskDefinition;
            Syntax   = syntax;
            Source   = source;
            EdgeMode = edgeMode;
            Target   = target;
            Triggers = triggers??new SymbolCollection<TriggerSymbol>();

            foreach (var trigger in Triggers) {
                trigger.Transition = this;
            }
        }

        [NotNull]
        public ITaskDefinitionSymbol TaskDefinition { get; }

        [NotNull]
        public Location Location {
            get { return Syntax.GetLocation(); }
        }

        [NotNull]
        public TransitionDefinitionSyntax Syntax { get; }
        
        [CanBeNull]
        public INodeReferenceSymbol Source { get; }

        [CanBeNull]
        public IEdgeModeSymbol EdgeMode { get; }

        [CanBeNull]
        public INodeReferenceSymbol Target { get; }
        
        [NotNull]
        public SymbolCollection<TriggerSymbol> Triggers { get; }

        IReadOnlySymbolCollection<ITriggerSymbol> ITransition.Triggers {
            get { return Triggers; }
        }

        [NotNull]
        public IEnumerable<ISymbol> Symbols() {

            if(Source != null) {
                yield return Source;
            }

            if (EdgeMode != null) {
                yield return EdgeMode;
            }

            if (Target != null) {
                yield return Target;
            }

            foreach(var trigger in Triggers) {
                yield return trigger;
            }
        }
    }
}