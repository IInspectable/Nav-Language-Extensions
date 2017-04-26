#region Using Directives

using System.Linq;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.Composition;

using Microsoft.VisualStudio.Language.Intellisense;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.CodeFixes {
    [ExportCodeFixActionProvider(nameof(IntroduceChoiceProvider))]
    class IntroduceChoiceProvider : CodeFixActionProvider {

        [ImportingConstructor]
        public IntroduceChoiceProvider(CodeFixActionContext context) : base(context) {
        }

        public override IEnumerable<SuggestedActionSet> GetSuggestedActions(CodeFixActionsParameter parameter, CancellationToken cancellationToken) {

            var choiceNodeSymbols = NodeReferenceFinder.FindRelatedNodeReferences(parameter.SymbolsInRange);

            var actions = choiceNodeSymbols.Select(nodeReference => new IntroduceChoiceAction(
                nodeReference   : nodeReference,
                parameter       : parameter,
                context         : Context));

            var actionSets = actions.Select(action => new SuggestedActionSet(
                actions         : new[] {action}, 
                title           : null, 
                priority        : SuggestedActionSetPriority.Medium, 
                applicableToSpan: action.ApplicableToSpan));

            return actionSets;
        }

        static class NodeReferenceFinder  {

            public static IEnumerable<INodeReferenceSymbol> FindRelatedNodeReferences(ImmutableList<ISymbol> symbols) {

                return symbols.OfType<INodeReferenceSymbol>()
                              .Where(nodeReference => nodeReference.Type == NodeReferenceType.Target)
                              .Where(nodeReference => nodeReference.Declaration         != null )
                              .Where(nodeReference => nodeReference.Transition.Source   != null)
                              .Where(nodeReference => nodeReference.Transition.EdgeMode != null)
                              .Where(nodeReference => !(nodeReference.Declaration is IChoiceNodeSymbol));                
            }            
        }
    }
}