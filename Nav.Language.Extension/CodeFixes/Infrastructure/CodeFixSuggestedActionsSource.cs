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
using Pharmatechnik.Nav.Language.Extension.Common;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.CodeFixes {
    partial class CodeFixSuggestedActionsSource : SemanticModelServiceDependent, ISuggestedActionsSource {

        readonly ICodeFixActionProviderService _codeFixActionProviderService;
        readonly ITextView _textView;

        volatile SuggestedActionSetsAndRange _cachedSuggestedActionSets;

        public CodeFixSuggestedActionsSource(ITextBuffer textBuffer, ICodeFixActionProviderService codeFixActionProviderService, ITextView textView)
            : base(textBuffer) {
            _codeFixActionProviderService = codeFixActionProviderService;
            _textView = textView;
            _textView.Caret.PositionChanged += OnCaretPositionChanged;
        }

        void OnCaretPositionChanged(object sender, CaretPositionChangedEventArgs e) {
            var caretPosition = _textView.Caret.Position.BufferPosition;
            if(IsCacheValid(_cachedSuggestedActionSets, caretPosition)) {
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

            var cachedActionSets = _cachedSuggestedActionSets;
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
            _cachedSuggestedActionSets = null;
            InvalidateSuggestedActions();
        }
        
        static bool IsCacheValid(SuggestedActionSetsAndRange cache, SnapshotPoint point) {
            if (cache == null) {
                return false;
            }
            if (cache.Range.Snapshot != point.Snapshot) {
                return false;
            }
            return cache.Range.Contains(point);
        }

        void InvalidateSuggestedActions() {
            _cachedSuggestedActionSets = null;
            SuggestedActionsChanged?.Invoke(this, EventArgs.Empty);
        }

        protected ImmutableList<SuggestedActionSet> BuildSuggestedActions(SnapshotPoint caretPoint, CancellationToken cancellationToken) {

            var codeGenerationUnitAndSnapshot = SemanticModelService?.CodeGenerationUnitAndSnapshot;

            if (codeGenerationUnitAndSnapshot == null || !codeGenerationUnitAndSnapshot.IsCurrent(caretPoint.Snapshot)) {
                _cachedSuggestedActionSets = null;
                return ImmutableList<SuggestedActionSet>.Empty;
            }
            
            var parameter  = new CodeFixActionsParameter(caretPoint, _textView, codeGenerationUnitAndSnapshot);
            var actionsets = BuildSuggestedActions(parameter, cancellationToken);

            if (cancellationToken.IsCancellationRequested || actionsets.Count==0) {
                return ImmutableList<SuggestedActionSet>.Empty;
            }

            var symbolRange = GetRange(parameter, codeGenerationUnitAndSnapshot);
            var actionsetsAndRange = new SuggestedActionSetsAndRange(symbolRange, actionsets);

            _cachedSuggestedActionSets = actionsetsAndRange;

            return actionsetsAndRange.SuggestedActionSets;
        }

        // TODO Passt das so?
        private static SnapshotSpan GetRange(CodeFixActionsParameter parameter, CodeGenerationUnitAndSnapshot codeGenerationUnitAndSnapshot) {
            var symbol = parameter.CodeFixContext.TryFindSymbolAtPosition();
            if (symbol == null) {
                return new SnapshotSpan(codeGenerationUnitAndSnapshot.Snapshot, 0, 0);
            } else {
                return symbol.GetSnapshotSpan(codeGenerationUnitAndSnapshot.Snapshot);
            }
        }

        protected ImmutableList<SuggestedActionSet> BuildSuggestedActions(CodeFixActionsParameter codeFixActionsParameter, CancellationToken cancellationToken) {

            var prios = new[] {
                SuggestedActionSetPriority.High,
                SuggestedActionSetPriority.Medium,
                SuggestedActionSetPriority.Low,
                SuggestedActionSetPriority.None
            };

            var groupedActions   = _codeFixActionProviderService.GetSuggestedActions(codeFixActionsParameter, cancellationToken)
                                                                .GroupBy(action => action.ApplicableToSpan)
                                                                .OrderBy(g => g.Key, SpanLengthComparer.Default).ToList();
            var prioIndex = 0;
            var actionSets = new List<SuggestedActionSet>();
            foreach ( var actionsInSpan in groupedActions) {
                actionSets.Add(new SuggestedActionSet(actionsInSpan, actionsInSpan.Key, prios[prioIndex]));
                prioIndex = Math.Min(prios.Length - 1, prioIndex + 1);
            }

            return actionSets.ToImmutableList();
        }
       
        sealed class SuggestedActionSetsAndRange {
            public SuggestedActionSetsAndRange(SnapshotSpan range, ImmutableList<SuggestedActionSet> suggestedActionSets) {
                Range = range;
                SuggestedActionSets = suggestedActionSets;
            }

            public SnapshotSpan Range { get; }
            public ImmutableList<SuggestedActionSet> SuggestedActionSets { get; }
        }
    }
}