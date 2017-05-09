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

        IEnumerable<IIncludeSymbol> GetCandidates() {
            return CodeGenerationUnit.Includes.Where(i => !i.TaskDeklarations.SelectMany(td => td.References).Any());
        }
        
        public IList<TextChange> GetTextChanges() {

            var textChanges = new List<TextChange?>();
            foreach (var candidate in GetCandidates()) {
                // TODO Wirklich der FullExtent?
                textChanges.Add(TryRemove(candidate.Syntax.GetFullExtent()));
            }            
            return textChanges.OfType<TextChange>().ToList();
        }     
    }
}