#region Using Directives

using System;
using System.Linq;
using System.Collections.Generic;

using Pharmatechnik.Nav.Language.Text;

#endregion

namespace Pharmatechnik.Nav.Language.CodeFixes {

    public class RemoveSignalTriggerQuotationMarksCodeFix : CodeFix {

        internal RemoveSignalTriggerQuotationMarksCodeFix(TransitionDefinitionBlockSyntax transitionDefinitionBlock, CodeFixContext context)
            : base(context) {
            TransitionDefinitionBlock = transitionDefinitionBlock ?? throw new ArgumentNullException(nameof(transitionDefinitionBlock));
        }
        
        public override string Name              => "Remove Quotation Marks from Trigger Names";
        public override CodeFixImpact Impact     => CodeFixImpact.None;
        public override TextExtent? ApplicableTo => null;
        public TransitionDefinitionBlockSyntax TransitionDefinitionBlock { get; }
       
        internal bool CanApplyFix() {
            return GetCanditates().Any();
        }

        IEnumerable<StringLiteralSyntax> GetCanditates() {
            return TransitionDefinitionBlock.TransitionDefinitions
                                            .Select(t => t.Trigger)
                                            .OfType<SignalTriggerSyntax>()
                                            .SelectMany(t => t.IdentifierOrStringList)
                                            .OfType<StringLiteralSyntax>();
        }


        public IList<TextChange> GetTextChanges() {
           
            var textChanges = new List<TextChange?>();

            foreach(var stringLiteralSyntax in GetCanditates()) {
                textChanges.Add(TryRemove(TextExtent.FromBounds(stringLiteralSyntax.Start, stringLiteralSyntax.Start + 1)));
                textChanges.Add(TryRemove(TextExtent.FromBounds(stringLiteralSyntax.End-1, stringLiteralSyntax.End)));
            }
            return textChanges.OfType<TextChange>().ToList();
        }
    }
}