using System;
using System.Collections.Generic;

namespace Pharmatechnik.Nav.Language {
    [Serializable]
    [SampleSyntax("")]
    public partial class TransitionDefinitionBlockSyntax : SyntaxNode {

        readonly IReadOnlyList<TransitionDefinitionSyntax>     _transitionDefinitions;
        readonly IReadOnlyList<ExitTransitionDefinitionSyntax> _exitTransitionDefinitions;

        internal TransitionDefinitionBlockSyntax(TextExtent extent, 
            IReadOnlyList<TransitionDefinitionSyntax> transitionDefinitions,
             IReadOnlyList<ExitTransitionDefinitionSyntax> exitTransitionDefinitions) 
            : base(extent) {

            AddChildNodes(_transitionDefinitions     = transitionDefinitions);
            AddChildNodes(_exitTransitionDefinitions = exitTransitionDefinitions);
        }

        public IReadOnlyList<TransitionDefinitionSyntax> TransitionDefinitions {
            get { return _transitionDefinitions; }
        }

        public IReadOnlyList<ExitTransitionDefinitionSyntax> ExitTransitionDefinitions {
            get { return _exitTransitionDefinitions; }
        }
    }
}