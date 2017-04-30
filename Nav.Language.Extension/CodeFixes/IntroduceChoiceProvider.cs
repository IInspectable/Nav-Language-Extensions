#region Using Directives

using System.Linq;
using System.Threading;
using System.Collections.Generic;
using System.ComponentModel.Composition;

using Microsoft.VisualStudio.Language.Intellisense;
using Pharmatechnik.Nav.Language.CodeFixes;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.CodeFixes {

    [ExportCodeFixActionProvider(nameof(IntroduceChoiceProvider))]
    class IntroduceChoiceProvider : CodeFixActionProvider {

        [ImportingConstructor]
        public IntroduceChoiceProvider(CodeFixActionContext context) : base(context) {
        }

        public override IEnumerable<SuggestedActionSet> GetSuggestedActions(CodeFixActionsParameter parameter, CancellationToken cancellationToken) {

            var codeFixes = FindCodeFixes(parameter);

            var actions = codeFixes.Select(codeFix => new IntroduceChoiceAction(
                codeFix         : codeFix,
                parameter       : parameter,
                context         : Context));

            var actionSets = actions.Select(action => new SuggestedActionSet(
                actions         : new[] {action}, 
                title           : null, 
                priority        : SuggestedActionSetPriority.Medium, 
                applicableToSpan: action.ApplicableToSpan));

            return actionSets;
        }

        public static IEnumerable<IntroduceChoiceCodeFix> FindCodeFixes(CodeFixActionsParameter parameter) {
            return parameter.Symbols
                            .OfType<INodeReferenceSymbol>()
                            .Select(nodeReference => new IntroduceChoiceCodeFix(parameter.GetEditorSettings(), parameter.CodeGenerationUnitAndSnapshot.CodeGenerationUnit, nodeReference))
                            .Where(fix => fix.CanApplyFix());
        }        
    }
}