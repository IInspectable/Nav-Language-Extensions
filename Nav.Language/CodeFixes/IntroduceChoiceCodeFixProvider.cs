#region Using Directives

using System.Linq;
using System.Threading;
using System.Collections.Generic;

#endregion

namespace Pharmatechnik.Nav.Language.CodeFixes {

    public static class IntroduceChoiceCodeFixProvider {
        
        public static IEnumerable<IntroduceChoiceCodeFix> SuggestCodeFixes(CodeFixContext context, CancellationToken cancellationToken) {

            var symbol = context.TryFindSymbolAtPosition();
            if (symbol == null) {
                return Enumerable.Empty<IntroduceChoiceCodeFix>();
            }

            var visitor = new Visitor(context);

            return visitor.Visit(symbol).Where(choiceCodeFix => choiceCodeFix.CanApplyFix());
        }

        sealed class Visitor : SymbolVisitor<IEnumerable<IntroduceChoiceCodeFix>> {
            
            public Visitor(CodeFixContext context) {
                Context = context;
            }

            CodeFixContext Context { get; }

            protected override IEnumerable<IntroduceChoiceCodeFix> DefaultVisit(ISymbol symbol) {
                yield break;
            }

            public override IEnumerable<IntroduceChoiceCodeFix> VisitNodeReferenceSymbol(INodeReferenceSymbol nodeReferenceSymbol) {
                // TODO Context weiterreichen
                yield return new IntroduceChoiceCodeFix(nodeReferenceSymbol, Context);
            }
        }
    }
}