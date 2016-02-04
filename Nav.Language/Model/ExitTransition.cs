using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Pharmatechnik.Nav.Language {

    sealed class ExitTransition : IExitTransition {
        
        internal ExitTransition(ExitTransitionDefinitionSyntax syntax, INodeReferenceSymbol source, IConnectionPointReferenceSymbol connectionPoint, IEdgeModeSymbol edgeMode, INodeReferenceSymbol target) {

            if (syntax == null) {
                throw new ArgumentNullException(nameof(syntax));
            }

            Syntax          = syntax;
            Source          = source;
            ConnectionPoint = connectionPoint;
            EdgeMode        = edgeMode;
            Target          = target;
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