﻿#region Using Directives

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

        readonly ICodeFixActionProviderService _codeFixActionProviderService;
        readonly ITextView _textView;

        volatile SuggestedActionSetsAndRange _cachedSuggestedActionSets;

        public CodeFixSuggestedActionsSource(ITextBuffer textBuffer, ICodeFixActionProviderService codeFixActionProviderService, ITextView textView)
            : base(textBuffer) {
            _codeFixActionProviderService = codeFixActionProviderService;
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
            IEnumerable<SuggestedActionSet> suggestedActionSets;
            if(IsCacheValid(cachedActionSets, range)) {                
                suggestedActionSets = cachedActionSets.SuggestedActionSets;
            } else {
                suggestedActionSets = BuildSuggestedActions(range, cancellationToken);
            }

            // TODO Sortierung funktioniert nicht...
            var orderedSuggestionSets= suggestedActionSets.OrderBy(s => s, new SuggestedActionSetComparer(caretPoint));

            return orderedSuggestionSets;
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

        protected ImmutableList<SuggestedActionSet> BuildSuggestedActions(SnapshotSpan range, CancellationToken cancellationToken) {

            var codeGenerationUnitAndSnapshot = SemanticModelService?.CodeGenerationUnitAndSnapshot;
            if (codeGenerationUnitAndSnapshot == null || !codeGenerationUnitAndSnapshot.IsCurrent(range.Snapshot)) {
                _cachedSuggestedActionSets = null;
                return ImmutableList<SuggestedActionSet>.Empty;
            }
            
            var parameter  = new CodeFixActionsParameter(range, codeGenerationUnitAndSnapshot, _textView);
            var actionsets = BuildSuggestedActions(parameter, cancellationToken);

            if (cancellationToken.IsCancellationRequested || actionsets.Count==0) {
                return ImmutableList<SuggestedActionSet>.Empty;
            }

            var actionsetsAndRange = new SuggestedActionSetsAndRange(range, actionsets);

            _cachedSuggestedActionSets = actionsetsAndRange;

            return actionsetsAndRange.SuggestedActionSets;
        }
        
        protected ImmutableList<SuggestedActionSet> BuildSuggestedActions(CodeFixActionsParameter codeFixActionsParameter, CancellationToken cancellationToken) {

            // TODO: Mach diese Gruppierung Sinn?
            var groupedActions   = _codeFixActionProviderService.GetSuggestedActions(codeFixActionsParameter, cancellationToken)
                                                                .GroupBy(action => action.ApplicableToSpan);
            var actionSets = new List<SuggestedActionSet>();
            foreach ( var actionsInSpan in groupedActions) {
                actionSets.Add(new SuggestedActionSet(actionsInSpan, applicableToSpan: actionsInSpan.Key/*, prios[prioIndex]*/));
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