#region Using Directives

using System.Linq;
using System.Threading;
using System.Collections.Generic;

#endregion

namespace Pharmatechnik.Nav.Language.CodeFixes {

    public sealed class RemoveUnusedIncludeDirectiveCodeFixProvider {

        public static IEnumerable<RemoveUnusedIncludeDirectiveCodeFix> SuggestCodeFixes(CodeFixContext context, CancellationToken cancellationToken) {

            // Wir schlagen den Codefix nur vor, wenn sich das Caret in einer Task Declaration befindet
            if (!context.FindNodes<IncludeDirectiveSyntax>().Any()) {
                yield break;
            }

            var codeFix = new RemoveUnusedIncludeDirectiveCodeFix(context);
            if (codeFix.CanApplyFix()) {
                yield return codeFix;
            }
        }

    }

}