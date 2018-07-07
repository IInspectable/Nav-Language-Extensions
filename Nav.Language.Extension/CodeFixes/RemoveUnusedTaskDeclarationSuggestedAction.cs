#region Using Directives

using System.Threading;

using Microsoft.VisualStudio.Imaging.Interop;

using Pharmatechnik.Nav.Language.CodeFixes.StyleFix;
using Pharmatechnik.Nav.Language.Extension.Images;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.CodeFixes {

    class RemoveUnusedTaskDeclarationSuggestedAction : CodeFixSuggestedAction<RemoveUnusedTaskDeclarationCodeFix> {

        public RemoveUnusedTaskDeclarationSuggestedAction(RemoveUnusedTaskDeclarationCodeFix codeFix,
                                                          CodeFixSuggestedActionParameter parameter,
                                                          CodeFixSuggestedActionContext context)
            : base(context, parameter, codeFix) {
        }

        public override ImageMoniker IconMoniker => ImageMonikers.RemoveUnusedSymbol;
        public override string DisplayText       => CodeFix.Name;

        protected override void Apply(CancellationToken cancellationToken) {

            ApplyTextChanges(CodeFix.GetTextChanges());                
        }
    }
}