#region Using Directives

using System.Linq;
using System.Threading;
using System.Collections.Generic;
using System.ComponentModel.Composition;

using Pharmatechnik.Nav.Language.CodeFixes;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.CodeFixes {

    [ExportCodeFixActionProvider(nameof(RemoveUnnecessaryQuotationsSuggestedActionProvider))]
    class RemoveUnnecessaryQuotationsSuggestedActionProvider : CodeFixActionProvider {

        [ImportingConstructor]
        public RemoveUnnecessaryQuotationsSuggestedActionProvider(CodeFixActionContext context) : base(context) {
        }

        public override IEnumerable<CodeFixSuggestedAction> GetSuggestedActions(CodeFixActionsParameter parameter, CancellationToken cancellationToken) {

            var editorSettings = parameter.GetEditorSettings();
            var codeGenerationUnitAndSnapshot = parameter.CodeGenerationUnitAndSnapshot.CodeGenerationUnit;
            var codeFixes = RemoveUnnecessaryQuotationsCodeFix.TryGetCodeFixes(parameter.OriginatingNode, codeGenerationUnitAndSnapshot, editorSettings);

            var actions = codeFixes.Select(codeFix => new RemoveUnnecessaryQuotationsSuggestedAction(
                codeFix  : codeFix,
                parameter: parameter,
                context  : Context));

            return actions;
        }   
    }
}