#region Using Directives

using System.Linq;
using System.Threading;
using System.Collections.Generic;

#endregion

namespace Pharmatechnik.Nav.Language.CodeFixes {

    public sealed class RemoveUnusedNodesCodeFixProvider {

        public static IEnumerable<RemoveUnusedNodesCodeFix> SuggestCodeFixes(CodeFixContext context, CancellationToken cancellationToken) {

            // Wir schlagen den Codefix nur vor, wenn sich das Caret in einer Node Declaration befindet
            var nodeDeclarationSyntax = context.FindNodes<NodeDeclarationSyntax>().FirstOrDefault();

            // Von dieser Node Declaration laufen wir hoch zum ganzen Block
            var nodeDeclarationBlock = nodeDeclarationSyntax?.AncestorsAndSelf().OfType<NodeDeclarationBlockSyntax>().FirstOrDefault();
            if (nodeDeclarationBlock == null) {
                yield break;
            }

            // Und von dort zur Task Definition
            var taskDefinition = context.CodeGenerationUnit.TaskDefinitions.FirstOrDefault(td => td.Syntax.NodeDeclarationBlock == nodeDeclarationBlock);
            if (taskDefinition == null) {
                // Sollte eigentlich nicht möglich sein
                yield break;
            }
            var codeFix = new RemoveUnusedNodesCodeFix(taskDefinition, context);
            if (codeFix.CanApplyFix()) {
                yield return codeFix;
            }
        }
    }
}