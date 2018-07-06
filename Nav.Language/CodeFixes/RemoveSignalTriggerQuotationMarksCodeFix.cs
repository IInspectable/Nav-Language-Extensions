#region Using Directives

using System.Linq;
using System.Collections.Generic;

using Pharmatechnik.Nav.Language.Text;

#endregion

namespace Pharmatechnik.Nav.Language.CodeFixes {

    public class RemoveSignalTriggerQuotationMarksCodeFix : CodeFix {

        internal RemoveSignalTriggerQuotationMarksCodeFix(CodeFixContext context)
            : base(context) {
        }
        
        public override string Name              => "Remove Quotation Marks from Trigger Names";
        public override CodeFixImpact Impact     => CodeFixImpact.None;
        public override TextExtent? ApplicableTo => null;
        public override CodeFixPrio Prio         => CodeFixPrio.Low;
        public override CodeFixCategory Category => CodeFixCategory.StyleFix;

        internal bool CanApplyFix() {
            return GetCanditates().Any();
        }

        IEnumerable<StringLiteralSyntax> GetCanditates() {
            return Syntax.DescendantNodes<TransitionDefinitionBlockSyntax>()
                         .SelectMany(transitionDefinitionBlockSyntax => transitionDefinitionBlockSyntax.TransitionDefinitions)
                         .Select(transitionDefinitionSyntax => transitionDefinitionSyntax.Trigger)
                         .OfType<SignalTriggerSyntax>()
                         .SelectMany(signalTriggerSyntax => signalTriggerSyntax.IdentifierOrStringList)
                         .OfType<StringLiteralSyntax>();
        }

        public IList<TextChange> GetTextChanges() {
           
            var textChanges = new List<TextChange>();

            foreach(var stringLiteralSyntax in GetCanditates()) {
                textChanges.AddRange(GetRemoveChanges(TextExtent.FromBounds(stringLiteralSyntax.Start, stringLiteralSyntax.Start + 1)));
                textChanges.AddRange(GetRemoveChanges(TextExtent.FromBounds(stringLiteralSyntax.End-1, stringLiteralSyntax.End)));
            }
            return textChanges;
        }
    }
}