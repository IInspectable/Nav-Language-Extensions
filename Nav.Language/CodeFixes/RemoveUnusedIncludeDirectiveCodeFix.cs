#region Using Directives

using System.Linq;
using System.Collections.Generic;
using Pharmatechnik.Nav.Language.Text;

#endregion

namespace Pharmatechnik.Nav.Language.CodeFixes {

    public class RemoveUnusedIncludeDirectiveCodeFix : CodeFix {

        internal RemoveUnusedIncludeDirectiveCodeFix(CodeFixContext context)
            : base(context) {
        }
        
        public override string Name              => "Remove Unused Taskref Directive";
        public override CodeFixImpact Impact     => CodeFixImpact.None;
        public override TextExtent? ApplicableTo => null;
       
        internal bool CanApplyFix() {
            return GetCandidates().Any();
        }

        IEnumerable<IncludeDirectiveSyntax> GetCandidates() {

            var includeDirectiveSyntaxes=SyntaxTree.GetRoot().DescendantNodes<IncludeDirectiveSyntax>();
            foreach (var includeDirectiveSyntax in includeDirectiveSyntaxes) {
               var includeSymbol = CodeGenerationUnit.Includes.FirstOrDefault(i => i.Syntax == includeDirectiveSyntax);
                if (includeSymbol == null || !includeSymbol.TaskDeklarations.SelectMany(td => td.References).Any()) {
                    yield return includeDirectiveSyntax;
                }               
            }
        }
        
        public IList<TextChange> GetTextChanges() {

            var textChanges = new List<TextChange?>();
            foreach (var candidate in GetCandidates()) {
                // TODO Wirklich der FullExtent?
                textChanges.Add(TryRemove(candidate.GetFullExtent()));
            }            
            return textChanges.OfType<TextChange>().ToList();
        }     
    }
}