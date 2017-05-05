#region Using Directives

using System.Linq;
using System.Threading;
using System.Collections.Generic;
using System.ComponentModel.Composition;

using Pharmatechnik.Nav.Language.CodeFixes;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.CodeFixes {

    [ExportCodeFixActionProvider(nameof(AddMissingExitTransitionSuggestedActionProvider))]
    class AddMissingExitTransitionSuggestedActionProvider : CodeFixActionProvider {

        [ImportingConstructor]
        public AddMissingExitTransitionSuggestedActionProvider(CodeFixActionContext context) : base(context) {
        }

        public override IEnumerable<CodeFixSuggestedAction> GetSuggestedActions(CodeFixActionsParameter parameter, CancellationToken cancellationToken) {

            var codeFixes = AddMissingExitTransitionCodeFixProvider.SuggestCodeFixes(parameter.CodeFixContext, cancellationToken);

            var actions = codeFixes.Select(codeFix => new AddMissingExitTransitionSuggestedAction(
                codeFix  : codeFix,
                parameter: parameter,
                context  : Context));
         
            return actions;
        }       
    }
}