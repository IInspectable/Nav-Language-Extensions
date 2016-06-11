#region Using Directives

using System;
using System.Collections.Generic;

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.CSharp.GoToNav {

    sealed class GoToNavAdornmentTagger : IntraTextAdornmentTagger<GoToNavTag, GoToNavAdornment> {
        
        internal static ITagger<IntraTextAdornmentTag> GetTagger(IWpfTextView view, Lazy<ITagAggregator<GoToNavTag>> colorTagger) {
            return view.Properties.GetOrCreateSingletonProperty(
                () => new GoToNavAdornmentTagger(view, colorTagger.Value));
        }

        readonly ITagAggregator<GoToNavTag> _colorTagger;

        GoToNavAdornmentTagger(IWpfTextView textView, ITagAggregator<GoToNavTag> colorTagger)
            : base(textView) {
            _colorTagger = colorTagger;
        }

        public void Dispose() {
            _colorTagger.Dispose();

            TextView.Properties.RemoveProperty(typeof(GoToNavAdornmentTagger));
        }

        // To produce adornments that don't obscure the text, the adornment tags
        // should have zero length spans. Overriding this method allows control
        // over the tag spans.
        protected override IEnumerable<Tuple<SnapshotSpan, PositionAffinity?, GoToNavTag>> GetAdornmentData(NormalizedSnapshotSpanCollection spans) {
            if(spans.Count == 0)
                yield break;

            ITextSnapshot snapshot = spans[0].Snapshot;

            var colorTags = _colorTagger.GetTags(spans);

            foreach(IMappingTagSpan<GoToNavTag> dataTagSpan in colorTags) {
                NormalizedSnapshotSpanCollection colorTagSpans = dataTagSpan.Span.GetSpans(snapshot);

                // Ignore data tags that are split by projection.
                // This is theoretically possible but unlikely in current scenarios.
                if(colorTagSpans.Count != 1)
                    continue;

                SnapshotSpan adornmentSpan = new SnapshotSpan(colorTagSpans[0].End, 0);

                yield return Tuple.Create(adornmentSpan, (PositionAffinity?) PositionAffinity.Successor, dataTagSpan.Tag);
            }
        }

        protected override GoToNavAdornment CreateAdornment(GoToNavTag dataTag, SnapshotSpan span) {
            return new GoToNavAdornment(dataTag, TextView);
        }

        protected override bool UpdateAdornment(GoToNavAdornment adornment, GoToNavTag dataTag) {
            adornment.Update(dataTag);
            return true;
        }
    }
}