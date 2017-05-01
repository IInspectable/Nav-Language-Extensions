#region Using Directives

using System.Linq;
using System.Threading;
using System.Collections.Generic;
using System.ComponentModel.Composition;

using Pharmatechnik.Nav.Language.CodeFixes;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.CodeFixes {

    [ExportCodeFixActionProvider(nameof(IntroduceChoiceSuggestedActionProvider))]
    class IntroduceChoiceSuggestedActionProvider : CodeFixActionProvider {

        [ImportingConstructor]
        public IntroduceChoiceSuggestedActionProvider(CodeFixActionContext context) : base(context) {
        }

        public override IEnumerable<CodeFixSuggestedAction> GetSuggestedActions(CodeFixActionsParameter parameter, CancellationToken cancellationToken) {

            var codeFixes = FindCodeFixes(parameter);

            var actions = codeFixes.Select(codeFix => new IntroduceChoiceSuggestedAction(
                codeFix  : codeFix,
                parameter: parameter,
                context  : Context));

            return actions;
        }

        public static IEnumerable<IntroduceChoiceCodeFix> FindCodeFixes(CodeFixActionsParameter parameter) {
            return parameter.Symbols
                            .OfType<INodeReferenceSymbol>()
                            .Select(nodeReference => new IntroduceChoiceCodeFix(parameter.GetEditorSettings(), parameter.CodeGenerationUnitAndSnapshot.CodeGenerationUnit, nodeReference))
                            .Where(fix => fix.CanApplyFix());
        }        
    }
}