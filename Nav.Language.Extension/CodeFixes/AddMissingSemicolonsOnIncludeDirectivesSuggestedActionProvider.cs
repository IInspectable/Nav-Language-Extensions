#region Using Directives

using System.Linq;
using System.Threading;
using System.Collections.Generic;
using System.ComponentModel.Composition;

using Pharmatechnik.Nav.Language.CodeFixes;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.CodeFixes {

    [ExportCodeFixActionProvider(nameof(AddMissingSemicolonsOnIncludeDirectivesSuggestedActionProvider))]
    class AddMissingSemicolonsOnIncludeDirectivesSuggestedActionProvider : CodeFixActionProvider {

        [ImportingConstructor]
        public AddMissingSemicolonsOnIncludeDirectivesSuggestedActionProvider(CodeFixActionContext context) : base(context) {
        }

        public override IEnumerable<CodeFixSuggestedAction> GetSuggestedActions(CodeFixActionsParameter parameter, CancellationToken cancellationToken) {

            var editorSettings = parameter.GetEditorSettings();
            var codeGenerationUnitAndSnapshot = parameter.CodeGenerationUnitAndSnapshot.CodeGenerationUnit;
            var codeFixes = AddMissingSemicolonsOnIncludeDirectivesCodeFixProvider.SuggestCodeFixes(parameter.OriginatingNode, codeGenerationUnitAndSnapshot, editorSettings);

            var actions = codeFixes.Select(codeFix => new AddMissingSemicolonsOnIncludeDirectivesSuggestedAction(
                codeFix  : codeFix,
                parameter: parameter,
                context  : Context));

            return actions;
        }   
    }
}