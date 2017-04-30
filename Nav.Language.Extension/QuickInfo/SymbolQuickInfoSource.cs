#region Using Directives

using System.Collections.Generic;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.QuickInfo {

    sealed class SymbolQuickInfoSource : SemanticModelServiceDependent, IQuickInfoSource {
        
        public SymbolQuickInfoSource(ITextBuffer textBuffer, 
                                     SyntaxQuickinfoBuilderService syntaxQuickinfoBuilderService) : base(textBuffer) {

            SyntaxQuickinfoBuilderService = syntaxQuickinfoBuilderService;
        }

        public SyntaxQuickinfoBuilderService SyntaxQuickinfoBuilderService { get;}
        
        public void AugmentQuickInfoSession(IQuickInfoSession session, IList<object> qiContent, out ITrackingSpan applicableToSpan) {
            applicableToSpan = null;

            var codeGenerationUnitAndSnapshot = SemanticModelService.CodeGenerationUnitAndSnapshot;
            if (codeGenerationUnitAndSnapshot == null) {
               return;
            }

            // Map the trigger point down to our buffer.
            SnapshotPoint? subjectTriggerPoint = session.GetTriggerPoint(codeGenerationUnitAndSnapshot.Snapshot);
            if (!subjectTriggerPoint.HasValue) {
                return;
            }
           
            var triggerSymbol = codeGenerationUnitAndSnapshot.CodeGenerationUnit.Symbols.FindAtPosition(subjectTriggerPoint.Value.Position);

            if (triggerSymbol == null) {
                return;
            }

            foreach(var content in SymbolQuickInfoBuilder.Build(triggerSymbol, SyntaxQuickinfoBuilderService)) {                 
                qiContent.Add(content);
            }
            
            var location = triggerSymbol.Location;
            applicableToSpan = codeGenerationUnitAndSnapshot.Snapshot.CreateTrackingSpan(
                    location.Start,
                    location.Length,
                    SpanTrackingMode.EdgeExclusive);
        }
    }
}