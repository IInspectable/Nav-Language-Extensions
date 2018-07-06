#region Using Directives

using System.Linq;
using System.Threading;
using System.Collections.Generic;

#endregion

namespace Pharmatechnik.Nav.Language.CodeFixes {

    public sealed class RemoveSignalTriggerQuotationMarksCodeFixProvider {

        public static IEnumerable<RemoveSignalTriggerQuotationMarksCodeFix> SuggestCodeFixes(CodeFixContext context, CancellationToken cancellationToken) {

            // Wir schlagen den Codefix nur vor, wenn sich das Caret in einer Transition mit Triggern befindet
            if (!context.FindNodes<TransitionDefinitionSyntax>().Any(td => td.Trigger is SignalTriggerSyntax)) {
                yield break;
            }

            var codeFix = new RemoveSignalTriggerQuotationMarksCodeFix(context);
            if (codeFix.CanApplyFix()) {
                yield return codeFix;
            }
        }

    }

}