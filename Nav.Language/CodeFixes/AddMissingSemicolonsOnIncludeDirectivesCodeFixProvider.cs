#region Using Directives

using System.Linq;
using System.Collections.Generic;
using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language.CodeFixes {
    public class AddMissingSemicolonsOnIncludeDirectivesCodeFixProvider {

        public static IEnumerable<AddMissingSemicolonsOnIncludeDirectivesCodeFix> SuggestCodeFixes([CanBeNull] SyntaxNode syntaxNode, CodeGenerationUnit codeGenerationUnit, EditorSettings editorSettings) {

            // Wir schlagen den Codefix nur vor, wenn sich das Caret in einer IncludeDirectiveSyntax befindet
            if (syntaxNode == null ||
                !syntaxNode.AncestorsAndSelf()
                    .OfType<IncludeDirectiveSyntax>()
                    .Any()) {
                yield break;
            }
            
            if (!syntaxNode.AncestorsAndSelf().OfType<IncludeDirectiveSyntax>().Any()) {
                yield break;
            }

            var codeFix = new AddMissingSemicolonsOnIncludeDirectivesCodeFix(codeGenerationUnit, editorSettings);
            if (codeFix.CanApplyFix()) {
                yield return codeFix;
            }
        }
    }
}