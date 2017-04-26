#region Using Directives

using System.Linq;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.Composition;

using Microsoft.VisualStudio.Language.Intellisense;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.CodeFixes {
    [ExportCodeFixActionProvider(nameof(RenameChoiceActionProvider))]
    class RenameChoiceActionProvider : CodeFixActionProvider {

        [ImportingConstructor]
        public RenameChoiceActionProvider(CodeFixActionContext context) : base(context) {
        }

        public override IEnumerable<SuggestedActionSet> GetSuggestedActions(CodeFixActionsParameter parameter, CancellationToken cancellationToken) {

            var choiceNodeSymbols = ChoiceNodeFinder.FindRelatedChoiceNodes(parameter.Symbols);

            var actions = choiceNodeSymbols.Select(choiceNodeSymbol => new RenameChoiceAction(
                choiceSymbol    : choiceNodeSymbol,
                parameter       : parameter,
                context         : Context));

            var actionSets = actions.Select(action => new SuggestedActionSet(
                actions         : new[] {action}, 
                title           : "Rename Choice", 
                priority        : SuggestedActionSetPriority.Medium, 
                applicableToSpan: action.ApplicableToSpan));

            return actionSets;
        }

        sealed class ChoiceNodeFinder : SymbolVisitor<IChoiceNodeSymbol> {

            public static IEnumerable<IChoiceNodeSymbol> FindRelatedChoiceNodes(ImmutableList<ISymbol> symbols) {
                var finder = new ChoiceNodeFinder();
                return symbols.Select(finder.Visit).Where(choiceNodeSymbol => choiceNodeSymbol != null);                
            }

            public override IChoiceNodeSymbol VisitChoiceNodeSymbol(IChoiceNodeSymbol choiceNodeSymbol) {
                return choiceNodeSymbol;
            }

            public override IChoiceNodeSymbol VisitNodeReferenceSymbol(INodeReferenceSymbol nodeReferenceSymbol) {
                return nodeReferenceSymbol.Declaration == null ? DefaultVisit(nodeReferenceSymbol) : Visit(nodeReferenceSymbol.Declaration);
            }
        }
    }
}