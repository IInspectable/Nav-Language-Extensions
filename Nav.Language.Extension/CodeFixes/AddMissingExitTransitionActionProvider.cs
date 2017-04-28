#region Using Directives

using System.Linq;
using System.Threading;
using System.Collections.Generic;
using System.ComponentModel.Composition;

using Microsoft.VisualStudio.Language.Intellisense;

using Pharmatechnik.Nav.Language.CodeFixes;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.CodeFixes {

    [ExportCodeFixActionProvider(nameof(AddMissingExitTransitionActionProvider))]
    class AddMissingExitTransitionActionProvider : CodeFixActionProvider {

        [ImportingConstructor]
        public AddMissingExitTransitionActionProvider(CodeFixActionContext context) : base(context) {
        }

        public override IEnumerable<SuggestedActionSet> GetSuggestedActions(CodeFixActionsParameter parameter, CancellationToken cancellationToken) {

            var codeFixes = FindCodeFixes(parameter);

            var actions = codeFixes.Select(codeFix => new AddMissingExitTransitionAction(
                codeFix         : codeFix,
                parameter       : parameter,
                context         : Context));

            var actionSets = actions.Select(action => new SuggestedActionSet(
                actions         : new[] {action},
                title           : null,
                priority        : SuggestedActionSetPriority.High,
                applicableToSpan: action.ApplicableToSpan));

            return actionSets;
        }

        public static IEnumerable<AddMissingExitTransitionCodeFix> FindCodeFixes(CodeFixActionsParameter parameter) {

            var targetNodes = parameter.Symbols.OfType<INodeReferenceSymbol>().Where(nr => nr.Type == NodeReferenceType.Target);

            foreach (var targetNode in targetNodes) {

                var taskNode = targetNode.Declaration as ITaskNodeSymbol;
                if (taskNode == null) {
                    continue;
                }

                foreach (var connectionPoint in GetMissingExitTransitionConnectionPoints(taskNode)) {

                    var codeFix = new AddMissingExitTransitionCodeFix(parameter.GetEditorSettings(), parameter.SemanticModelResult.CodeGenerationUnit, targetNode, connectionPoint);
                    if (codeFix.CanApplyFix()) {
                        yield return codeFix;
                    }
                }
            }           
        }

        // TODO Evtl in ITaskNodeSymbol respektive TaskNodeSymbolExtensions
       static  IEnumerable<IConnectionPointSymbol> GetMissingExitTransitionConnectionPoints(ITaskNodeSymbol taskNode) {
            if (taskNode.Declaration == null) {
                yield break;
            }
            var expectedExits = taskNode.Declaration.Exits();
            var actualExits = taskNode.Outgoings
                                      .Select(et => et.ConnectionPoint)
                                      .Where(cp => cp != null)
                                      .ToList();

            foreach (var expectedExit in expectedExits) {

                if (!actualExits.Exists(cpRef => cpRef.Declaration == expectedExit)) {

                    yield return expectedExit;
                }
            }
        }
    }
}