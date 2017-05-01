#region Using Directives

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Immutable;

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text.Editor;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.CodeFixes {

    class CodeFixSuggestedActionsSource : SemanticModelServiceDependent, ISuggestedActionsSource {

        readonly ICodeFixActionProviderService _codeFixActionProviderService;
        readonly ITextView _textView;

        volatile ActionSetsWithRange _cachedActionSets;

        public CodeFixSuggestedActionsSource(ITextBuffer textBuffer, ICodeFixActionProviderService codeFixActionProviderService, ITextView textView)
            : base(textBuffer) {
            _codeFixActionProviderService = codeFixActionProviderService;
            _textView = textView;
            _textView.Caret.PositionChanged += OnCaretPositionChanged;
        }

        void OnCaretPositionChanged(object sender, CaretPositionChangedEventArgs e) {
            var caretPosition = _textView.Caret.Position.BufferPosition;
            if(IsCacheValid(_cachedActionSets, caretPosition)) {
                return;
            }
            InvalidateSuggestedActions();
        }

        public override void Dispose() {
            base.Dispose();
            _textView.Caret.PositionChanged -= OnCaretPositionChanged;
        }

        public event EventHandler<EventArgs> SuggestedActionsChanged;

        public bool TryGetTelemetryId(out Guid telemetryId) {
            telemetryId = Guid.Empty;
            return false;
        }

        public IEnumerable<SuggestedActionSet> GetSuggestedActions(ISuggestedActionCategorySet requestedActionCategories, SnapshotSpan range, CancellationToken cancellationToken) {

            var cachedActionSets = _cachedActionSets;
            var caretPosition    =_textView.Caret.Position.BufferPosition;

            if (IsCacheValid(cachedActionSets, caretPosition)) {
                return cachedActionSets.SuggestedActionSets;
            }

            return BuildSuggestedActions(caretPosition, cancellationToken);
        }

        public Task<bool> HasSuggestedActionsAsync(ISuggestedActionCategorySet requestedActionCategories, SnapshotSpan range, CancellationToken cancellationToken) {
            var caretPosition = _textView.Caret.Position.BufferPosition;
            return Task.Factory.StartNew(() => BuildSuggestedActions(caretPosition, cancellationToken).Any(), cancellationToken);
        }

        protected override void OnSemanticModelChanged(object sender, SnapshotSpanEventArgs e) {
            base.OnSemanticModelChanged(sender, e);
            _cachedActionSets = null;
            InvalidateSuggestedActions();
        }
        
        static bool IsCacheValid(ActionSetsWithRange cache, SnapshotPoint point) {
            if (cache == null) {
                return false;
            }
            if (cache.Range.Snapshot != point.Snapshot) {
                return false;
            }
            return cache.Range.Contains(point);
        }

        void InvalidateSuggestedActions() {
            _cachedActionSets = null;
            SuggestedActionsChanged?.Invoke(this, EventArgs.Empty);
        }

        protected ImmutableList<SuggestedActionSet> BuildSuggestedActions(SnapshotPoint caretPoint, CancellationToken cancellationToken) {

            var codeGenerationUnitAndSnapshot = SemanticModelService?.CodeGenerationUnitAndSnapshot;

            if (codeGenerationUnitAndSnapshot == null || !codeGenerationUnitAndSnapshot.IsCurrent(caretPoint.Snapshot)) {
                _cachedActionSets = null;
                return ImmutableList<SuggestedActionSet>.Empty;
            }

            var symbols    = FindSymbols(caretPoint, codeGenerationUnitAndSnapshot).ToImmutableList();
            var parameter  = new CodeFixActionsParameter(symbols, codeGenerationUnitAndSnapshot, _textView);
            var actionsets = BuildSuggestedActions(parameter, cancellationToken);

            if (cancellationToken.IsCancellationRequested) {
                return ImmutableList<SuggestedActionSet>.Empty;
            }

            var symbolRange         = ToSnapshotSpan(caretPoint.Snapshot, symbols);
            var actionsetsWithRange = new ActionSetsWithRange(symbolRange, actionsets);
            _cachedActionSets = actionsetsWithRange;

            return actionsetsWithRange.SuggestedActionSets;
        }

        SnapshotSpan ToSnapshotSpan(ITextSnapshot textSnapshot, ImmutableList<ISymbol> symbols) {
            if(symbols.Count == 0) {
                return new SnapshotSpan(textSnapshot, 0, 0);
            }
            var start = symbols.First().Start;
            var end   = symbols.Last().End;

            return new SnapshotSpan(textSnapshot, start, end - start);
        }

        protected ImmutableList<SuggestedActionSet> BuildSuggestedActions(CodeFixActionsParameter codeFixActionsParameter, CancellationToken cancellationToken) {

            var suggestedActions = _codeFixActionProviderService.GetSuggestedActions(codeFixActionsParameter, cancellationToken);
            var actionSets = new List<SuggestedActionSet>();
            foreach( var actionsInSpan in suggestedActions.GroupBy(action=> action.ApplicableToSpan)) {
                actionSets.Add(new SuggestedActionSet(actionsInSpan, actionsInSpan.Key));
            }

            return actionSets.ToImmutableList();
        }

        static IEnumerable<ISymbol> FindSymbols(SnapshotPoint caretPoint, CodeGenerationUnitAndSnapshot codeGenerationUnitAndSnapshot) {

            var symbol = codeGenerationUnitAndSnapshot.CodeGenerationUnit.Symbols.FindAtPosition(caretPoint.Position);
            if(symbol == null && caretPoint.Position > 0) {
                symbol = codeGenerationUnitAndSnapshot.CodeGenerationUnit.Symbols.FindAtPosition(caretPoint.Position-1);
            }

            if(symbol == null) {
                yield break;
            }
            yield return symbol;
        }

        sealed class ActionSetsWithRange {
            public ActionSetsWithRange(SnapshotSpan range, ImmutableList<SuggestedActionSet> suggestedActionSets) {
                Range = range;
                SuggestedActionSets = suggestedActionSets;
            }

            public SnapshotSpan Range { get; }
            public ImmutableList<SuggestedActionSet> SuggestedActionSets { get; }
        }
    }
}