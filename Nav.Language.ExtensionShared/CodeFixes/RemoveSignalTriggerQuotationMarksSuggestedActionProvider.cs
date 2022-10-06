#region Using Directives

using System.Linq;
using System.Threading;
using System.Collections.Generic;
using System.ComponentModel.Composition;

using Pharmatechnik.Nav.Language.CodeFixes.StyleFix;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.CodeFixes; 

[ExportCodeFixSuggestedActionProvider(nameof(RemoveSignalTriggerQuotationMarksSuggestedActionProvider))]
class RemoveSignalTriggerQuotationMarksSuggestedActionProvider: CodeFixSuggestedActionProvider {

    [ImportingConstructor]
    public RemoveSignalTriggerQuotationMarksSuggestedActionProvider(CodeFixSuggestedActionContext context): base(context) {
    }

    public override IEnumerable<CodeFixSuggestedAction> GetSuggestedActions(CodeFixSuggestedActionParameter parameter, CancellationToken cancellationToken) {

        var codeFixes = RemoveSignalTriggerQuotationMarksCodeFixProvider.SuggestCodeFixes(parameter.CodeFixContext, cancellationToken);

        var actions = codeFixes.Select(codeFix => new RemoveSignalTriggerQuotationMarksSuggestedAction(
                                           codeFix  : codeFix,
                                           parameter: parameter,
                                           context  : Context));

        return actions;
    }

}