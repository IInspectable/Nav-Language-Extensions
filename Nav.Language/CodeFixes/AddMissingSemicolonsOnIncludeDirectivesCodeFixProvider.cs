#region Using Directives

using System.Threading;
using System.Collections.Generic;

#endregion

namespace Pharmatechnik.Nav.Language.CodeFixes {
    public class AddMissingSemicolonsOnIncludeDirectivesCodeFixProvider {

        public static IEnumerable<AddMissingSemicolonsOnIncludeDirectivesCodeFix> SuggestCodeFixes(CodeFixContext context, CancellationToken cancellationToken) {
            // Wir schlagen den Codefix nur vor, wenn sich das Caret in einer IncludeDirectiveSyntax befindet
            if (!context.ContainsNodes<IncludeDirectiveSyntax>()) {
                yield break;
            }
            
            var codeFix = new AddMissingSemicolonsOnIncludeDirectivesCodeFix(context);
            if (codeFix.CanApplyFix()) {
                yield return codeFix;
            }
        }
    }
}