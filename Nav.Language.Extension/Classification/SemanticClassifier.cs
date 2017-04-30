#region Using Directives

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Pharmatechnik.Nav.Language.Extension.Common;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Classification {
        
    sealed class SemanticClassifier: SemanticModelServiceDependent, IClassifier {

        SemanticClassifier(IClassificationTypeRegistryService classificationTypeRegistryService, ITextBuffer textBuffer) : base(textBuffer) {
            ClassificationTypeRegistryService = classificationTypeRegistryService;
        }

        public static IClassifier GetOrCreateSingelton(IClassificationTypeRegistryService classificationTypeRegistryService, ITextBuffer textBuffer) {
            return new TextBufferScopedClassifier(
                textBuffer,
                typeof(SemanticClassifier), () =>
                   new SemanticClassifier(classificationTypeRegistryService, textBuffer));
        }

        public IClassificationTypeRegistryService ClassificationTypeRegistryService { get; }

        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span) {
            var result = new List<ClassificationSpan>();
            
            var codeGenerationUnitAndSnapshot = SemanticModelService.CodeGenerationUnitAndSnapshot;
            if(codeGenerationUnitAndSnapshot == null) {
                return result;
            }

            var classificationType = ClassificationTypeRegistryService.GetClassificationType(ClassificationTypeNames.DeadCode);

            var extent      = TextExtent.FromBounds(span.Start.Position, span.End.Position);
            var diagnostics = codeGenerationUnitAndSnapshot.CodeGenerationUnit.Diagnostics;
            var candidates  = diagnostics.Where(diagnostic => diagnostic.Category == DiagnosticCategory.DeadCode)
                                         .Where(d => d.Location.Extent.IntersectsWith(extent)); 

            foreach (var diagnostic in candidates) {
                var diagnosticSpan = new SnapshotSpan(codeGenerationUnitAndSnapshot.Snapshot, new Span(diagnostic.Location.Start, diagnostic.Location.Length));

                var classification = new ClassificationSpan(
                        diagnosticSpan.TranslateTo(span.Snapshot, SpanTrackingMode.EdgeExclusive),
                        classificationType);


                result.Add(classification);
            }
            
            return result;
        }

        protected override void OnSemanticModelChanged(object sender, SnapshotSpanEventArgs e) {
            ClassificationChanged?.Invoke(this, new ClassificationChangedEventArgs(e.Span));
        }
        
        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;
    }
}