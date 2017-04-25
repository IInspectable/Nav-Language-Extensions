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
    class RenameChoiceActionProvider : ICodeFixActionProvider {

        readonly CodeFixActionContext _context;
        
        [ImportingConstructor]
        public RenameChoiceActionProvider(CodeFixActionContext context) {
            _context = context;
        }

        public IEnumerable<ISuggestedAction> GetSuggestedActions(CodeFixActionsArgs codeFixActionsArgs, CancellationToken cancellationToken) {

            var choiceNodeSymbols = ChoiceNodeFinder.FindRelatedChoiceNodes(codeFixActionsArgs.SymbolsInRange);

            return choiceNodeSymbols.Select(choiceNodeSymbol => new RenameChoiceAction(
                choiceSymbol      : choiceNodeSymbol,
                codeFixActionsArgs: codeFixActionsArgs,
                context           : _context));              
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