#region Using Directives

using System.Linq;
using System.Threading;
using System.Collections.Generic;

#endregion

namespace Pharmatechnik.Nav.Language.CodeFixes {

    public sealed class RemoveUnusedTaskDeclarationCodeFixProvider {

        public static IEnumerable<RemoveUnusedTaskDeclarationCodeFix> SuggestCodeFixes(CodeFixContext context, CancellationToken cancellationToken) {

            // Wir schlagen den Codefix nur vor, wenn sich das Caret in einer Task Declaration befindet
            var taskDeclarationSyntax = context.FindNodes<SyntaxNode>()
                                               .Select(n => n.AncestorsAndSelf().OfType<TaskDeclarationSyntax>().FirstOrDefault())
                                               .FirstOrDefault(tds => tds!=null);

            if (taskDeclarationSyntax == null) {
                yield break;
            }

            // Und von dort zum Symbol
            var taskDeclaration = context.CodeGenerationUnit.TaskDeclarations.FirstOrDefault(td => td.Syntax == taskDeclarationSyntax);
            if (taskDeclaration == null) {
                // Sollte eigentlich nicht möglich sein
                yield break;
            }

            var codeFix = new RemoveUnusedTaskDeclarationCodeFix(taskDeclaration, context);
            if (codeFix.CanApplyFix()) {
                yield return codeFix;
            }
        }
    }
}