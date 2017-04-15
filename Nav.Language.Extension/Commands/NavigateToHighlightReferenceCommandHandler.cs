#region Using Directives

using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition;

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Outlining;
using Microsoft.VisualStudio.Text.Tagging;

using Pharmatechnik.Nav.Language.Extension.Common;
using Pharmatechnik.Nav.Language.Extension.Commands.Extensibility;
using Pharmatechnik.Nav.Language.Extension.HighlightReferences;

#endregion
// TODO Code Review

namespace Pharmatechnik.Nav.Language.Extension.Commands {

    [ExportCommandHandler("Navigate To Highlight Reference Command Handler", NavLanguageContentDefinitions.ContentType)]
    class NavigateToHighlightReferenceCommandHandler:
        ICommandHandler<NavigateToHighlightedReferenceCommandArgs> {

        readonly IViewTagAggregatorFactoryService _tagAggregatorFactory;
        readonly IOutliningManagerService _outliningManagerService;

        [ImportingConstructor]
        public NavigateToHighlightReferenceCommandHandler(IViewTagAggregatorFactoryService tagAggregatorFactory, IOutliningManagerService outliningManagerService) {
            _tagAggregatorFactory    = tagAggregatorFactory;
            _outliningManagerService = outliningManagerService;

        }

        public CommandState GetCommandState(NavigateToHighlightedReferenceCommandArgs args, Func<CommandState> nextHandler) {
            return CommandState.Available;
        }

        public void ExecuteCommand(NavigateToHighlightedReferenceCommandArgs args, Action nextHandler) {
            using(var tagger = _tagAggregatorFactory.CreateTagAggregator<ReferenceHighlightTag>(args.TextView)) {
                var tagUnderCursor = FindTagUnderCaret(tagger, args.TextView);

                if (tagUnderCursor == null) {
                    nextHandler();
                    return;
                }

                var spans = GetTags(tagger, args.TextView.TextSnapshot.GetFullSpan()).ToList();

                var destTag = GetDestinationTag(tagUnderCursor.Value, spans, args.Direction);
                if(args.TextView.TryMoveCaretToAndEnsureVisible(destTag.Start, _outliningManagerService)) {
                    args.TextView.SetSelection(destTag);
                }
            }
        }

        static IList<SnapshotSpan> GetTags(ITagAggregator<ReferenceHighlightTag> tagAggregator, SnapshotSpan span) {
            return tagAggregator.GetTags(span)
                                .SelectMany(tag => tag.Span.GetSpans(span.Snapshot.TextBuffer))
                                .OrderBy(tag => tag.Start)
                                .ToList();
        }

        static SnapshotSpan GetDestinationTag(
            SnapshotSpan tagUnderCursor,
            List<SnapshotSpan> orderedTagSpans,
            NavigateDirection direction) {

            var destIndex = orderedTagSpans.BinarySearch(tagUnderCursor, new StartComparer());

            destIndex += direction == NavigateDirection.Down ? 1 : -1;
            if (destIndex < 0) {
                destIndex = orderedTagSpans.Count - 1;
            } else if (destIndex == orderedTagSpans.Count) {
                destIndex = 0;
            }

            return orderedTagSpans[destIndex];
        }

        private SnapshotSpan? FindTagUnderCaret(
            ITagAggregator<ReferenceHighlightTag> tagAggregator,
            ITextView textView) {
            // We always want to be working with the surface buffer here, so this line is correct
            var caretPosition = textView.Caret.Position.BufferPosition.Position;

            var tags = GetTags(tagAggregator, new SnapshotSpan(textView.TextSnapshot, new Span(caretPosition, 0)));
            return tags.Any()
                ? tags.First()
                : (SnapshotSpan?)null;
        }

        class StartComparer : IComparer<SnapshotSpan> {
            public int Compare(SnapshotSpan x, SnapshotSpan y) {
                return x.Start.CompareTo(y.Start);
            }
        }
    }
}
