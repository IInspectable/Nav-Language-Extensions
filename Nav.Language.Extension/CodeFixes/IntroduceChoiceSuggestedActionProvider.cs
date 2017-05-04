#region Using Directives

using System.Linq;
using System.Threading;
using System.Collections.Generic;
using System.ComponentModel.Composition;

using Pharmatechnik.Nav.Language.CodeFixes;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.CodeFixes {

    [ExportCodeFixActionProvider(nameof(IntroduceChoiceSuggestedActionProvider))]
    class IntroduceChoiceSuggestedActionProvider : CodeFixActionProvider {

        [ImportingConstructor]
        public IntroduceChoiceSuggestedActionProvider(CodeFixActionContext context) : base(context) {
        }

        public override IEnumerable<CodeFixSuggestedAction> GetSuggestedActions(CodeFixActionsParameter parameter, CancellationToken cancellationToken) {

            var editorSettings = parameter.GetEditorSettings();
            var codeGenerationUnitAndSnapshot = parameter.CodeGenerationUnitAndSnapshot.CodeGenerationUnit;
            var codeFixes = IntroduceChoiceCodeFix.TryGetCodeFixes(parameter.OriginatingSymbol, codeGenerationUnitAndSnapshot, editorSettings);

            var actions = codeFixes.Select(codeFix => new IntroduceChoiceSuggestedAction(
                codeFix  : codeFix,
                parameter: parameter,
                context  : Context));

            return actions;
        }   
    }
}