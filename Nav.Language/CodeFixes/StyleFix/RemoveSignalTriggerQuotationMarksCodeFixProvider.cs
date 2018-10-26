#region Using Directives

using System.Collections.Generic;
using System.Linq;
using System.Threading;

#endregion

namespace Pharmatechnik.Nav.Language.CodeFixes.StyleFix {

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