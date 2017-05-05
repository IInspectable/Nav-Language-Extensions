#region Using Directives

using System.Linq;
using System.Collections.Generic;
using System.Threading;

#endregion

namespace Pharmatechnik.Nav.Language.CodeFixes {

    public sealed class RemoveSignalTriggerQuotationMarksCodeFixProvider {
        
        public static IEnumerable<RemoveSignalTriggerQuotationMarksCodeFix> SuggestCodeFixes(CodeFixContext context, CancellationToken cancellationToken) {

            // Wir schlagen den Codefix nur vor, wenn sich das Caret in einer Transition mit Triggern befindet
            var transition = context.FindNodes<TransitionDefinitionSyntax>().FirstOrDefault(td => td.Trigger is SignalTriggerSyntax);

            // Von dieser Transition laufen wir hoch zum ganzen Block
            var transitionDefinitionBlock = transition?.AncestorsAndSelf().OfType<TransitionDefinitionBlockSyntax>().FirstOrDefault();
            if (transitionDefinitionBlock == null) {
                yield break;
            }

            var codeFix= new RemoveSignalTriggerQuotationMarksCodeFix(transitionDefinitionBlock, context);
            if(codeFix.CanApplyFix()) {
                yield return codeFix;
            }
        }
    }
}