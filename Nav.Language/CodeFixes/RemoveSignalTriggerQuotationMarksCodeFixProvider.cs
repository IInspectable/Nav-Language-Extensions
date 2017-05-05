#region Using Directives

using System.Linq;
using System.Collections.Generic;
using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language.CodeFixes {

    public sealed class RemoveSignalTriggerQuotationMarksCodeFixProvider {
        
        public static IEnumerable<RemoveSignalTriggerQuotationMarksCodeFix> SuggestCodeFixes([CanBeNull] SyntaxNode syntaxNode, CodeGenerationUnit codeGenerationUnit, EditorSettings editorSettings) {
            
            // Wir schlagen den Codefix nur vor, wenn sich das Caret in einer Transition mit Triggern befindet
            if (syntaxNode ==null || 
                !syntaxNode.AncestorsAndSelf()
                           .OfType<TransitionDefinitionSyntax>()
                           .Any(td => td.Trigger is SignalTriggerSyntax)) {
                yield break;
            }

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