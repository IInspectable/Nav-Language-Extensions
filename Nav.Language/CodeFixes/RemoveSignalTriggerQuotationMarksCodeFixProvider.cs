#region Using Directives

using System.Linq;
using System.Collections.Generic;

#endregion

namespace Pharmatechnik.Nav.Language.CodeFixes {

    public sealed class RemoveSignalTriggerQuotationMarksCodeFixProvider {
        
        public static IEnumerable<RemoveSignalTriggerQuotationMarksCodeFix> TryGetCodeFixes(SyntaxNode syntaxNode, CodeGenerationUnit codeGenerationUnit, EditorSettings editorSettings) {

            var transitionDefinitionBlock = syntaxNode.AncestorsAndSelf().OfType<TransitionDefinitionBlockSyntax>().FirstOrDefault();
            if (transitionDefinitionBlock == null) {
                yield break;
            }

            var codeFix= new RemoveSignalTriggerQuotationMarksCodeFix(transitionDefinitionBlock, codeGenerationUnit, editorSettings);
            if(codeFix.CanApplyFix()) {
                yield return codeFix;
            }
        }
    }
}