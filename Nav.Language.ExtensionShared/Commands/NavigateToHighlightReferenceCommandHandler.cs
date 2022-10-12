#region Using Directives

using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition;

using Microsoft.VisualStudio.Commanding;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Outlining;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Text.Editor.Commanding.Commands;
using Microsoft.VisualStudio.Utilities;

using Pharmatechnik.Nav.Language.Extension.Common;
using Pharmatechnik.Nav.Language.Extension.HighlightReferences;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Commands; 

[Export(typeof(ICommandHandler))]
[ContentType(NavLanguageContentDefinitions.ContentType)]
[Name(CommandHandlerNames.NavigateToHighlightReferenceCommandHandler)]
class NavigateToHighlightReferenceCommandHandler:
    ICommandHandler<NavigateToNextHighlightedReferenceCommandArgs>,
    ICommandHandler<NavigateToPreviousHighlightedReferenceCommandArgs> {

    readonly IViewTagAggregatorFactoryService _tagAggregatorFactory;
    readonly IOutliningManagerService         _outliningManagerService;

    [ImportingConstructor]
    public NavigateToHighlightReferenceCommandHandler(IViewTagAggregatorFactoryService tagAggregatorFactory, IOutliningManagerService outliningManagerService) {
        _tagAggregatorFactory    = tagAggregatorFactory;
        _outliningManagerService = outliningManagerService;
    }

    public string DisplayName => "Go To Next/Previous Member";

    public CommandState GetCommandState(NavigateToPreviousHighlightedReferenceCommandArgs args) {
        return CommandState.Available;
    }

    public bool ExecuteCommand(NavigateToPreviousHighlightedReferenceCommandArgs args, CommandExecutionContext executionContext) {
        return ExecuteCommand(args.TextView, NavigateDirection.Up);
    }

    public CommandState GetCommandState(NavigateToNextHighlightedReferenceCommandArgs args) {
        return CommandState.Available;
    }

    public bool ExecuteCommand(NavigateToNextHighlightedReferenceCommandArgs args, CommandExecutionContext executionContext) {
        return ExecuteCommand(args.TextView, NavigateDirection.Down);
    }

    enum NavigateDirection {

        Up   = -1,
        Down = 1,

    }

    bool ExecuteCommand(ITextView textView, NavigateDirection direction) {
        var wpfTextView = textView as IWpfTextView;
        if (wpfTextView == null) {
            return false;
        }

        using var tagger         = _tagAggregatorFactory.CreateTagAggregator<ReferenceHighlightTag>(wpfTextView);
        var       tagUnderCursor = FindTagUnderCaret(tagger, wpfTextView);

        if (tagUnderCursor == null) {
            return false;
        }

        var spans = GetReferenceSpans(tagger, wpfTextView.TextSnapshot.GetFullSpan()).ToList();

        var destinationSpan = GetDestinationSpan(tagUnderCursor.Value, spans, direction);
        if (wpfTextView.TryMoveCaretToAndEnsureVisible(destinationSpan.Start, _outliningManagerService)) {
            wpfTextView.SetSelection(destinationSpan);
        }

        return true;

    }

    static IList<SnapshotSpan> GetReferenceSpans(ITagAggregator<ReferenceHighlightTag> tagAggregator, SnapshotSpan span) {
        return tagAggregator.GetTags(span)
                            .SelectMany(tag => tag.Span.GetSpans(span.Snapshot.TextBuffer))
                            .OrderBy(tag => tag.Start)
                            .ToList();
    }

    static SnapshotSpan GetDestinationSpan(SnapshotSpan tagUnderCursor, List<SnapshotSpan> orderedTagSpans, NavigateDirection direction) {

        var destIndex = orderedTagSpans.BinarySearch(tagUnderCursor, new StartComparer());

        destIndex += direction == NavigateDirection.Down ? 1 : -1;
        if (destIndex < 0) {
            destIndex = orderedTagSpans.Count - 1;
        } else if (destIndex == orderedTagSpans.Count) {
            destIndex = 0;
        }

        return orderedTagSpans[destIndex];
    }

    SnapshotSpan? FindTagUnderCaret(ITagAggregator<ReferenceHighlightTag> tagAggregator, ITextView textView) {
        // We always want to be working with the surface buffer here, so this line is correct
        var caretPosition = textView.Caret.Position.BufferPosition.Position;

        var tags = GetReferenceSpans(tagAggregator, new SnapshotSpan(textView.TextSnapshot, new Span(caretPosition, 0)));
        return tags.Any()
            ? tags.First()
            : null;
    }

    sealed class StartComparer: IComparer<SnapshotSpan> {

        public int Compare(SnapshotSpan x, SnapshotSpan y) {
            return x.Start.CompareTo(y.Start);
        }

    }

}