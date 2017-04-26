#region Using Directives

using System;
using System.Collections.Generic;

using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language {

    sealed class ExitTransition : IExitTransition {
        
        internal ExitTransition(ExitTransitionDefinitionSyntax syntax,
                                TaskDefinitionSymbol containingTask,
                                [CanBeNull] NodeReferenceSymbol source,
                                [CanBeNull] ConnectionPointReferenceSymbol connectionPoint,
                                [CanBeNull] EdgeModeSymbol edgeMode,
                                [CanBeNull] NodeReferenceSymbol target) {

            Syntax          = syntax         ?? throw new ArgumentNullException(nameof(syntax));
            ContainingTask  = containingTask ?? throw new ArgumentNullException(nameof(containingTask));
            Source          = source;
            ConnectionPoint = connectionPoint;
            EdgeMode        = edgeMode;
            Target          = target;

            if (source != null) {
                source.Edge = this;
            }
            if (edgeMode != null) {
                edgeMode.Edge = this;
            }
            if (target != null) {
                target.Edge = this;
            }
            if (connectionPoint != null) {                
                connectionPoint.ExitTransition = this;
            }
        }

        [NotNull]
        public ITaskDefinitionSymbol ContainingTask { get; }

        [NotNull]
        public Location Location {
            get { return Syntax.GetLocation(); }
        }

        [NotNull]
        public ExitTransitionDefinitionSyntax Syntax { get; }
        
        [CanBeNull]
        public INodeReferenceSymbol Source { get; }

        [CanBeNull]
        public IConnectionPointReferenceSymbol ConnectionPoint { get; }

        [CanBeNull]
        public IEdgeModeSymbol EdgeMode { get; }

        [CanBeNull]
        public INodeReferenceSymbol Target { get; }

        [NotNull]
        public IEnumerable<ISymbol> Symbols() {

            if (Source != null) {
                yield return Source;
            }

            if (ConnectionPoint != null) {
                yield return ConnectionPoint;
            }

            if (EdgeMode != null) {
                yield return EdgeMode;
            }

            if (Target != null) {
                yield return Target;
            }
        }
    }
}