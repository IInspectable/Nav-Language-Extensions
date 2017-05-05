#region Using Directives

using System.Linq;
using System.Collections.Generic;
using Pharmatechnik.Nav.Language.Text;

#endregion

namespace Pharmatechnik.Nav.Language.CodeFixes {
    public class AddMissingSemicolonsOnIncludeDirectivesCodeFix : CodeFix {

        internal AddMissingSemicolonsOnIncludeDirectivesCodeFix(CodeGenerationUnit codeGenerationUnit, EditorSettings editorSettings)
            : base(codeGenerationUnit, editorSettings) {
        }

        public override string Name => "Add missing ';' on Include Directives";
        public override CodeFixImpact Impact => CodeFixImpact.None;

        internal bool CanApplyFix() {
            return GetCanditates().Any();
        }

        IEnumerable<IncludeDirectiveSyntax> GetCanditates() {
            return Syntax.DescendantNodes<IncludeDirectiveSyntax>().Where(ids => ids.Semicolon.IsMissing);
        }

        public IList<TextChange> GetTextChanges() {

            var textChanges = new List<TextChange?>();

            foreach (var includeDirectiveSyntax in GetCanditates()) {
                textChanges.Add(TryInsert(includeDirectiveSyntax.End, SyntaxFacts.Semicolon));
            }
            return textChanges.OfType<TextChange>().ToList();
        }
    }
}
