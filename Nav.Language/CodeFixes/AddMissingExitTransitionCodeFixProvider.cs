#region Using Directives

using System.Linq;
using System.Threading;
using System.Collections.Generic;

#endregion

namespace Pharmatechnik.Nav.Language.CodeFixes {
    
    public static class AddMissingExitTransitionCodeFixProvider {
        
        public static IEnumerable<AddMissingExitTransitionCodeFix> SuggestCodeFixes(CodeFixContext context, CancellationToken cancellationToken) {

            var symbol = context.TryFindSymbolAtPosition();
            if (symbol == null) {
                return Enumerable.Empty<AddMissingExitTransitionCodeFix>();
            }

            var provider = new Visitor(context);

            return provider.Visit(symbol).Where(cf => cf.CanApplyFix());
        }

        sealed class Visitor : SymbolVisitor<IEnumerable<AddMissingExitTransitionCodeFix>> {
            
            public Visitor(CodeFixContext context) {
                Context = context;
            }

            CodeFixContext Context { get; }

            protected override IEnumerable<AddMissingExitTransitionCodeFix> DefaultVisit(ISymbol symbol) {
                yield break;
            }

            public override IEnumerable<AddMissingExitTransitionCodeFix> VisitNodeReferenceSymbol(INodeReferenceSymbol nodeReferenceSymbol) {

                // Add Missing Edge
                var taskNode = nodeReferenceSymbol.Declaration as ITaskNodeSymbol;
                if (taskNode != null) {
                    foreach (var missingExitConnectionPoint in taskNode.GetMissingExitTransitionConnectionPoints()) {
                        yield return new AddMissingExitTransitionCodeFix(nodeReferenceSymbol, missingExitConnectionPoint, Context);
                    }
                }
            }
        }
    }    
}