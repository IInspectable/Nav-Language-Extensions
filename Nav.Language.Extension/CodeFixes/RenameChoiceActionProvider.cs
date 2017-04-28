#region Using Directives

using System.Linq;
using System.Threading;
using System.Collections.Generic;
using System.ComponentModel.Composition;

using Microsoft.VisualStudio.Language.Intellisense;

using Pharmatechnik.Nav.Language.CodeFixes;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.CodeFixes {

    [ExportCodeFixActionProvider(nameof(RenameChoiceActionProvider))]
    class RenameChoiceActionProvider : CodeFixActionProvider {

        [ImportingConstructor]
        public RenameChoiceActionProvider(CodeFixActionContext context) : base(context) {
        }

        public override IEnumerable<SuggestedActionSet> GetSuggestedActions(CodeFixActionsParameter parameter, CancellationToken cancellationToken) {

            var choiceNodeSymbols = CodeFixFinder.FindCodeFixes(parameter);

            var actions = choiceNodeSymbols.Select(codeFix => new RenameChoiceAction(
                codeFix         : codeFix,
                parameter       : parameter,
                context         : Context));

            var actionSets = actions.Select(action => new SuggestedActionSet(
                actions         : new[] {action}, 
                title           : "Rename Choice", 
                priority        : SuggestedActionSetPriority.Medium, 
                applicableToSpan: action.ApplicableToSpan));

            return actionSets;
        }

        sealed class CodeFixFinder : SymbolVisitor<IChoiceNodeSymbol> {

            public static IEnumerable<RenameChoiceCodeFix> FindCodeFixes(CodeFixActionsParameter parameter) {
                var finder = new CodeFixFinder();
                return parameter.Symbols.Select(finder.Visit).Where(choiceNodeSymbol => choiceNodeSymbol != null)
                                .Select( choiceNode=> new RenameChoiceCodeFix(parameter.GetEditorSettings(), parameter.SemanticModelResult.CodeGenerationUnit, choiceNode))
                                .Where(codeFix=>codeFix.CanApplyFix());                
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