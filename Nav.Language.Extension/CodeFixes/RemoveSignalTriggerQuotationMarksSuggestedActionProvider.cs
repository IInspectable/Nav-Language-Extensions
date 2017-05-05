#region Using Directives

using System.Linq;
using System.Threading;
using System.Collections.Generic;
using System.ComponentModel.Composition;

using Pharmatechnik.Nav.Language.CodeFixes;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.CodeFixes {

    [ExportCodeFixActionProvider(nameof(RemoveSignalTriggerQuotationMarksSuggestedActionProvider))]
    class RemoveSignalTriggerQuotationMarksSuggestedActionProvider : CodeFixActionProvider {

        [ImportingConstructor]
        public RemoveSignalTriggerQuotationMarksSuggestedActionProvider(CodeFixActionContext context) : base(context) {
        }

        public override IEnumerable<CodeFixSuggestedAction> GetSuggestedActions(CodeFixActionsParameter parameter, CancellationToken cancellationToken) {

            var codeFixes = RemoveSignalTriggerQuotationMarksCodeFixProvider.SuggestCodeFixes(parameter.CodeFixContext, cancellationToken);

            var actions = codeFixes.Select(codeFix => new RemoveSignalTriggerQuotationMarksSuggestedAction(
                codeFix  : codeFix,
                parameter: parameter,
                context  : Context));

            return actions;
        }   
    }
}