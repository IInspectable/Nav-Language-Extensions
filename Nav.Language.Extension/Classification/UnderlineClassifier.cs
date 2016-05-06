#region Using Directives

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using Pharmatechnik.Nav.Language.Extension.Common;
using Pharmatechnik.Nav.Language.Extension.Underlining;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Classification {

    class UnderlineClassifier : IClassifier, IDisposable {

        public UnderlineClassifier(ITextBuffer textBuffer,
                               ITagAggregator<UnderlineTag> underlineTagAggregator,
                               IClassificationTypeRegistryService classificationTypeRegistryService) {
            TextBuffer = textBuffer;
            UnderlineTagAggregator = underlineTagAggregator;
            ClassificationTypeRegistryService = classificationTypeRegistryService;

            UnderlineTagAggregator.TagsChanged += OnUnderlineTagsChanged;
        }

        public void Dispose() {
            UnderlineTagAggregator.TagsChanged -= OnUnderlineTagsChanged;
        }

        void OnUnderlineTagsChanged(object sender, TagsChangedEventArgs e) {
            ClassificationChanged?.Invoke(this, new ClassificationChangedEventArgs(e.Span.GetSpans(TextBuffer)[0]));
        }

        public static IClassifier GetOrCreateSingelton(IClassificationTypeRegistryService classificationTypeRegistryService, ITextBuffer textBuffer, ITagAggregator<UnderlineTag> underlineTagAggregator) {
            return new TextBufferScopedClassifier(
                textBuffer,
                typeof(UnderlineClassifier), () =>
                   new UnderlineClassifier(textBuffer, underlineTagAggregator, classificationTypeRegistryService));
        }
        
        public ITextBuffer TextBuffer { get; }
        public ITagAggregator<UnderlineTag> UnderlineTagAggregator { get; }
        public IClassificationTypeRegistryService ClassificationTypeRegistryService { get; }

        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;

        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span) {
            var result = new List<ClassificationSpan>();

            var classificationType = ClassificationTypeRegistryService.GetClassificationType(ClassificationTypeNames.Underline);
            
            foreach (var tagSpan in UnderlineTagAggregator.GetTags(span)) {
                var tagSpans = tagSpan.Span.GetSpans(span.Snapshot);
                result.Add(new ClassificationSpan(tagSpans[0], classificationType));
            }

            return result;
        }        
    }
}