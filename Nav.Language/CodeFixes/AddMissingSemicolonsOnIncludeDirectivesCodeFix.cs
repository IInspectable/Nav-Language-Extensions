#region Using Directives

using System.Linq;
using System.Collections.Generic;

using Pharmatechnik.Nav.Language.Text;

#endregion

namespace Pharmatechnik.Nav.Language.CodeFixes {
    public class AddMissingSemicolonsOnIncludeDirectivesCodeFix : CodeFix {

        internal AddMissingSemicolonsOnIncludeDirectivesCodeFix(CodeFixContext context)
            : base(context) {
        }

        public override string Name              => "Add missing ';' on Include Directives";
        public override CodeFixImpact Impact     => CodeFixImpact.None;
        public override TextExtent? ApplicableTo => null;

        internal bool CanApplyFix() {
            return GetCanditates().Any();
        }

        IEnumerable<IncludeDirectiveSyntax> GetCanditates() {
            return Syntax.DescendantNodes<IncludeDirectiveSyntax>().Where(ids => ids.Semicolon.IsMissing);
        }

        public IList<TextChange> GetTextChanges() {

            var textChanges = new List<TextChange>();

            foreach (var includeDirectiveSyntax in GetCanditates()) {
                textChanges.AddRange(GetInsertChanges(includeDirectiveSyntax.End, SyntaxFacts.Semicolon));
            }
            return textChanges;
        }
    }
}
