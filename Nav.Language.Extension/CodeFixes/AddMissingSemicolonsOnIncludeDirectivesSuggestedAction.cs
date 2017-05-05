#region Using Directives

using System.Threading;

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Imaging.Interop;
using Pharmatechnik.Nav.Language.CodeFixes;
using Pharmatechnik.Nav.Language.Extension.Images;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.CodeFixes {

    class AddMissingSemicolonsOnIncludeDirectivesSuggestedAction : CodeFixSuggestedAction<AddMissingSemicolonsOnIncludeDirectivesCodeFix> {

        public AddMissingSemicolonsOnIncludeDirectivesSuggestedAction(AddMissingSemicolonsOnIncludeDirectivesCodeFix codeFix,
                                                                      CodeFixActionsParameter parameter,
                                                                      CodeFixActionContext context)
            : base(context, parameter, codeFix) {
        }

        public override Span? ApplicableToSpan   => null;
        public override ImageMoniker IconMoniker => ImageMonikers.AddSemicolon;
        public override string DisplayText       => CodeFix.Name;

        protected override void Apply(CancellationToken cancellationToken) {

            ApplyTextChanges(CodeFix.GetTextChanges());                
        }
    }
}