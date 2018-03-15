#region Using Directives

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Immutable;

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Language.Intellisense;
using Pharmatechnik.Nav.Language.Extension.Common;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.CodeFixes {
    partial class CodeFixSuggestedActionsSource : SemanticModelServiceDependent, ISuggestedActionsSource {

        readonly ICodeFixSuggestedActionProviderService _codeFixSuggestedActionProviderService;
        readonly ITextView _textView;

        volatile SuggestedActionSetsAndRange _cachedSuggestedActionSets;

        public CodeFixSuggestedActionsSource(ITextBuffer textBuffer, ICodeFixSuggestedActionProviderService codeFixSuggestedActionProviderService, ITextView textView)
            : base(textBuffer) {
            _codeFixSuggestedActionProviderService = codeFixSuggestedActionProviderService;
            _textView = textView;
        }
        
        public event EventHandler<EventArgs> SuggestedActionsChanged;

        public bool TryGetTelemetryId(out Guid telemetryId) {
            telemetryId = Guid.Empty;
            return false;
        }

        public IEnumerable<SuggestedActionSet> GetSuggestedActions(ISuggestedActionCategorySet requestedActionCategories, SnapshotSpan range, CancellationToken cancellationToken) {

            var caretPoint = _textView.GetCaretPoint();
            var cachedActionSets = _cachedSuggestedActionSets;
            IEnumerable<CodeFixSuggestedAction> suggestedActionSets =
                IsCacheValid(cachedActionSets, range) ? cachedActionSets.SuggestedActionSets : BuildSuggestedActions(range, cancellationToken);

            // Nach Span Gruppieren
            var groupedActions = suggestedActionSets.GroupBy(action => action.ApplicableToSpan);
            var suggestedActionSet = new List<SuggestedActionSet>();
            foreach (var actionsInSpan in groupedActions) {
                var orderedActions = actionsInSpan.OrderByDescending(codeFixSuggestedAction => codeFixSuggestedAction.Prio);
                suggestedActionSet.Add(new SuggestedActionSet(orderedActions, applicableToSpan: actionsInSpan.Key ?? range));
            }
            
            // Sortierung nach Nähe zum Caret Point
            var orderedSuggestionSets = suggestedActionSet.OrderBy(s => s, new SuggestedActionSetComparer(caretPoint, range));
            // Doppelte Actions entfernen. Es bleibt nur die zum Caret nächste Action bestehen.
            var filteredSets = FilterDuplicateTitles(orderedSuggestionSets);

            return filteredSets;
        }

        IEnumerable<SuggestedActionSet> FilterDuplicateTitles(IEnumerable<SuggestedActionSet> actionSets) {

            var result = new List<SuggestedActionSet>();

            var seenTitles = new HashSet<string>();

            foreach (var set in actionSets) {
                var filteredSet = FilterDuplicateTitles(set, seenTitles);
                if (filteredSet != null) {
                    result.Add(filteredSet);
                }
            }

            return result.ToImmutableArray();
        }

        SuggestedActionSet FilterDuplicateTitles(SuggestedActionSet actionSet, HashSet<string> seenTitles) {

            var actions = new List<ISuggestedAction>();

            foreach(var action in actionSet.Actions) {
                if (seenTitles.Add(action.DisplayText)) {
                    actions.Add(action);
                }
            }

            return actions.Count == 0
                ? null
                : new SuggestedActionSet(actions, actionSet.Title, actionSet.Priority, actionSet.ApplicableToSpan);
        }

        public Task<bool> HasSuggestedActionsAsync(ISuggestedActionCategorySet requestedActionCategories, SnapshotSpan range, CancellationToken cancellationToken) {
            return Task.Factory.StartNew(() => BuildSuggestedActions(range, cancellationToken).Any(), cancellationToken);
        }

        protected override void OnSemanticModelChanged(object sender, SnapshotSpanEventArgs e) {
            base.OnSemanticModelChanged(sender, e);
            _cachedSuggestedActionSets = null;
            InvalidateSuggestedActions();
        }
        
        static bool IsCacheValid(SuggestedActionSetsAndRange cache, SnapshotSpan range) {
            if (cache == null) {
                return false;
            }
            
            return cache.Range == range;
        }

        void InvalidateSuggestedActions() {
            _cachedSuggestedActionSets = null;
            SuggestedActionsChanged?.Invoke(this, EventArgs.Empty);
        }

        protected ImmutableList<CodeFixSuggestedAction> BuildSuggestedActions(SnapshotSpan range, CancellationToken cancellationToken) {

            var codeGenerationUnitAndSnapshot = SemanticModelService?.CodeGenerationUnitAndSnapshot;
            if (codeGenerationUnitAndSnapshot == null || !codeGenerationUnitAndSnapshot.IsCurrent(range.Snapshot)) {
                _cachedSuggestedActionSets = null;
                return ImmutableList<CodeFixSuggestedAction>.Empty;
            }
            
            var parameter  = new CodeFixSuggestedActionParameter(range, codeGenerationUnitAndSnapshot, _textView);
            var actionsets = _codeFixSuggestedActionProviderService.GetCodeFixSuggestedActions(parameter, cancellationToken).ToImmutableList();

            if (cancellationToken.IsCancellationRequested || actionsets.Count==0) {
                return ImmutableList<CodeFixSuggestedAction>.Empty;
            }

            var actionsetsAndRange = new SuggestedActionSetsAndRange(range, actionsets);

            _cachedSuggestedActionSets = actionsetsAndRange;

            return actionsetsAndRange.SuggestedActionSets;
        }
               
        sealed class SuggestedActionSetsAndRange {
            public SuggestedActionSetsAndRange(SnapshotSpan range, ImmutableList<CodeFixSuggestedAction> suggestedActionSets) {
                Range = range;
                SuggestedActionSets = suggestedActionSets;
            }

            public SnapshotSpan Range { get; }
            public ImmutableList<CodeFixSuggestedAction> SuggestedActionSets { get; }
        }
    }
}