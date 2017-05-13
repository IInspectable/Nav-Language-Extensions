#region Using Directives

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Pharmatechnik.Nav.Language.Extension.Common;
using Pharmatechnik.Nav.Language.Extension.Underlining;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Classification {

    class UnderlineClassifier : ITagger<ClassificationTag>, IDisposable {

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
            TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(e.Span.GetSpans(TextBuffer)[0]));
        }

        public static ITagger<T> GetOrCreateSingelton<T>(IClassificationTypeRegistryService classificationTypeRegistryService, ITextView textView, ITextBuffer buffer, ITagAggregator<UnderlineTag> underlineTagAggregator) where T : ITag {            
            return GetOrCreateSingelton(classificationTypeRegistryService, textView, buffer, underlineTagAggregator) as ITagger<T>;            
        }

        public static UnderlineClassifier GetOrCreateSingelton(IClassificationTypeRegistryService classificationTypeRegistryService, ITextView textView, ITextBuffer buffer, ITagAggregator<UnderlineTag> underlineTagAggregator) {
            return textView.GetOrCreateAutoClosingProperty(v =>
                new UnderlineClassifier(buffer, underlineTagAggregator, classificationTypeRegistryService));
        }

        public ITextBuffer TextBuffer { get; }
        public ITagAggregator<UnderlineTag> UnderlineTagAggregator { get; }
        public IClassificationTypeRegistryService ClassificationTypeRegistryService { get; }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public IEnumerable<ITagSpan<ClassificationTag>> GetTags(NormalizedSnapshotSpanCollection spans) {

            var classificationType = ClassificationTypeRegistryService.GetClassificationType(ClassificationTypeNames.Underline);

            foreach(var span in spans) {
                foreach(var tagSpan in UnderlineTagAggregator.GetTags(span)) {
                    var tagSpans = tagSpan.Span.GetSpans(span.Snapshot);
                    yield return new TagSpan<ClassificationTag>(tagSpans[0], new ClassificationTag(classificationType));
                }
            }
        }
    }
}