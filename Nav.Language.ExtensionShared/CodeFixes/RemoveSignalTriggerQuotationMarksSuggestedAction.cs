#region Using Directives

using System.Threading;

using Microsoft.VisualStudio.Imaging.Interop;

using Pharmatechnik.Nav.Language.CodeFixes.StyleFix;
using Pharmatechnik.Nav.Language.Extension.Images;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.CodeFixes {

    class RemoveSignalTriggerQuotationMarksSuggestedAction: CodeFixSuggestedAction<RemoveSignalTriggerQuotationMarksCodeFix> {

        public RemoveSignalTriggerQuotationMarksSuggestedAction(RemoveSignalTriggerQuotationMarksCodeFix codeFix,
                                                                CodeFixSuggestedActionParameter parameter,
                                                                CodeFixSuggestedActionContext context)
            : base(context, parameter, codeFix) {
        }

        public override ImageMoniker IconMoniker => ImageMonikers.DeleteQuotationMarks;
        public override string       DisplayText => CodeFix.Name;

        protected override void Apply(CancellationToken cancellationToken) {

            ApplyTextChanges(CodeFix.GetTextChanges());
        }

    }

}