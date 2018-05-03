#region Using Directives

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Pharmatechnik.Nav.Language.Extension.Common;
using Pharmatechnik.Nav.Language.Text;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Classification {

    sealed class SyntaxClassifier : ParserServiceDependent, IClassifier {

        readonly Dictionary<SyntaxTokenClassification, IClassificationType> _classificationMap;

        SyntaxClassifier(IClassificationTypeRegistryService registry, ITextBuffer textBuffer) : base(textBuffer) {

            _classificationMap= ClassificationTypeDefinitions.GetSyntaxTokenClassificationMap(registry);
        }

        public static IClassifier GetOrCreateSingelton(IClassificationTypeRegistryService registry, ITextBuffer textBuffer) {
            return new TextBufferScopedClassifier(
                textBuffer,
                typeof(SyntaxClassifier), () =>
                   new SyntaxClassifier(registry, textBuffer));
        }
       
        protected override void OnParseResultChanged(object sender, SnapshotSpanEventArgs e) {          
            ClassificationChanged?.Invoke(this, new ClassificationChangedEventArgs(e.Span));
        }

        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;

        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span) {
            var result = new List<ClassificationSpan>();

            var syntaxTreeAndSnapshot = ParserService.SyntaxTreeAndSnapshot;
            if (syntaxTreeAndSnapshot == null) {
                return result;
            }

            var extent = TextExtent.FromBounds(span.Start.Position, span.End.Position);
            foreach(var token in syntaxTreeAndSnapshot.SyntaxTree.Tokens[extent, includeOverlapping: true]) {

                _classificationMap.TryGetValue(token.Classification, out var ct);
                if (ct == null) {
                    continue;
                }

                var tokenSpan = new SnapshotSpan(syntaxTreeAndSnapshot.Snapshot, new Span(token.Start, token.Length));
                
                var classification = new ClassificationSpan(
                        tokenSpan.TranslateTo(span.Snapshot, SpanTrackingMode.EdgeExclusive),
                        ct);

                result.Add(classification);
            }

            return result;
        }
    }
}