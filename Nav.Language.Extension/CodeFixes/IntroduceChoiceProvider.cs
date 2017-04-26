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

            var choiceNodeSymbols = NodeReferenceFinder.FindRelatedChoiceNodes(parameter.SymbolsInRange);

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

        sealed class NodeReferenceFinder : SymbolVisitor<INodeReferenceSymbol> {

            public static IEnumerable<INodeReferenceSymbol> FindRelatedChoiceNodes(ImmutableList<ISymbol> symbols) {
                var finder = new NodeReferenceFinder();
                return symbols.Select(finder.Visit).Where(nodeReference => nodeReference != null);                
            }

            public override INodeReferenceSymbol VisitNodeReferenceSymbol(INodeReferenceSymbol nodeReferenceSymbol) {

                var task = nodeReferenceSymbol.Declaration?.ContainingTask;
                // TODO an INodeReferenceSymbol dranhängen, ob Target oder Source
                if (task == null || nodeReferenceSymbol.Declaration is IChoiceNodeSymbol || 
                    (task.Transitions.All(t => t.Target != nodeReferenceSymbol) && task.ExitTransitions.All(t => t.Target != nodeReferenceSymbol))) {
                    return DefaultVisit(nodeReferenceSymbol);
                }
                return nodeReferenceSymbol;
            }
        }
    }
}