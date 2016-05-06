#region Using Directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Pharmatechnik.Nav.Language.Extension.Common;
using Pharmatechnik.Nav.Language.Extension.GoToDefinition;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Underlining {

    public class UnderlineTagger: ITagger<UnderlineTag> {

        readonly ITextBuffer _textBuffer;
        readonly List<SnapshotSpan> _underlineSpans;

        public UnderlineTagger(ITextBuffer textBuffer) {
            _underlineSpans=new List<SnapshotSpan>();
            _textBuffer = textBuffer;
        }

        public void AddUnderlineSpan(SnapshotSpan span) {
            if(_underlineSpans.Any(underlineSpan => underlineSpan == span)) {
                return;
            }

            _underlineSpans.Add(span);

            var args = new SnapshotSpanEventArgs(span);
            TagsChanged?.Invoke(this, args);
        }

        public void RemoveUnderlineSpan(SnapshotSpan span) {

            if(_underlineSpans.RemoveAll(s=> s == span) > 0) {
                var args = new SnapshotSpanEventArgs(span);
                TagsChanged?.Invoke(this, args);
            }            
        }

        public void RemoveAllUnderlineSpans() {
            if (_underlineSpans.Count == 0) {
                return;
            }

            _underlineSpans.Clear();
            var args = new SnapshotSpanEventArgs(_textBuffer.CurrentSnapshot.ToSnapshotSpan());
            TagsChanged?.Invoke(this, args);
        }

        public static UnderlineTagger GetOrCreateSingelton(ITextBuffer textBuffer) {

            return textBuffer.Properties.GetOrCreateSingletonProperty(
                    () => new UnderlineTagger(textBuffer)
                    );

        }

        public static ITagger<T> GetOrCreateSingelton<T>(ITextBuffer textBuffer) where T : ITag {
            return GetOrCreateSingelton(textBuffer) as ITagger<T>;
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public IEnumerable<ITagSpan<UnderlineTag>> GetTags(NormalizedSnapshotSpanCollection spans) {

            foreach (var span in spans) {
                foreach (var underlineSpan in _underlineSpans) {
                    if (span.IntersectsWith(underlineSpan)) {
                        var tagSpan = underlineSpan.TranslateTo(span.Snapshot, SpanTrackingMode.EdgeExclusive);
                        var tag     = new UnderlineTag();
                        yield return new TagSpan<UnderlineTag>(tagSpan, tag);
                    }
                }
            }
        }
    }
}