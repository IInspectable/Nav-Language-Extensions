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

using Pharmatechnik.Nav.Language.CodeFixes;
using Pharmatechnik.Nav.Language.Extension.Common;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.CodeFixes; 

//// ISuggestedActionsSource2
//class CatSet: ISuggestedActionCategorySet {

//    public CatSet() {
//        ISuggestedActionCategoryRegistryService d=null;
//        var set = d.CreateSuggestedActionCategorySet(PredefinedSuggestedActionCategoryNames.Refactoring);
//    }
//    public IEnumerator<string> GetEnumerator() {
//        yield return PredefinedSuggestedActionCategoryNames.Refactoring; //Schraubenzieher
//        yield return PredefinedSuggestedActionCategoryNames.ErrorFix;    // Birne mit Error
//        yield return PredefinedSuggestedActionCategoryNames.StyleFix;    // Birne
//        yield return PredefinedSuggestedActionCategoryNames.CodeFix;     // Birne
//    }

//    public bool Contains(string categoryName) {
//        return categoryName == null || PredefinedSuggestedActionCategoryNames.Refactoring == categoryName;
//    }

//    IEnumerator IEnumerable.GetEnumerator() {
//        return GetEnumerator();
//    }

//}

partial class CodeFixSuggestedActionsSource: SemanticModelServiceDependent, ISuggestedActionsSource2 {

    readonly ISuggestedActionCategoryRegistryService _suggestedActionCategoryRegistryService;
    readonly ICodeFixSuggestedActionProviderService  _codeFixSuggestedActionProviderService;
    readonly ITextView                               _textView;

    volatile SuggestedActionSetsAndRange _cachedSuggestedActionSets;

    public CodeFixSuggestedActionsSource(ITextBuffer textBuffer,
                                         ISuggestedActionCategoryRegistryService suggestedActionCategoryRegistryService,
                                         ICodeFixSuggestedActionProviderService codeFixSuggestedActionProviderService,
                                         ITextView textView)
        : base(textBuffer) {
        _suggestedActionCategoryRegistryService = suggestedActionCategoryRegistryService;
        _codeFixSuggestedActionProviderService  = codeFixSuggestedActionProviderService;
        _textView                               = textView;
    }

    public event EventHandler<EventArgs> SuggestedActionsChanged;

    public bool TryGetTelemetryId(out Guid telemetryId) {
        telemetryId = Guid.Empty;
        return false;
    }

    public Task<ISuggestedActionCategorySet> GetSuggestedActionCategoriesAsync(ISuggestedActionCategorySet requestedActionCategories, SnapshotSpan range, CancellationToken cancellationToken) {

        return Task.Factory.StartNew(GetSuggestedActionCategorySet,
                                     cancellationToken,
                                     TaskCreationOptions.None, TaskScheduler.Default);

        ISuggestedActionCategorySet GetSuggestedActionCategorySet() {

            var actions = GetOrCreateFixSuggestedActions(range, cancellationToken);

            var categories = actions.GroupBy(a => a.Category)
                                    .Select(g => g.Key)
                                    .ToList();

            if (!requestedActionCategories.Contains(PredefinedSuggestedActionCategoryNames.Refactoring)) {
                categories.Remove(CodeFixCategory.Refactoring);
                categories.Remove(CodeFixCategory.StyleFix);
            }

            var suggestions = categories.Select(ToCategoryName);

            return _suggestedActionCategoryRegistryService.CreateSuggestedActionCategorySet(suggestions);
        }

    }

    string ToCategoryName(CodeFixCategory category) {
        switch (category) {
            case CodeFixCategory.CodeFix:
                return PredefinedSuggestedActionCategoryNames.CodeFix;
            case CodeFixCategory.ErrorFix:
                return PredefinedSuggestedActionCategoryNames.ErrorFix;
            case CodeFixCategory.StyleFix:
                return PredefinedSuggestedActionCategoryNames.StyleFix;
            case CodeFixCategory.Refactoring:
                return PredefinedSuggestedActionCategoryNames.Refactoring;
            default:
                return PredefinedSuggestedActionCategoryNames.Any;
        }
    }

    public Task<bool> HasSuggestedActionsAsync(ISuggestedActionCategorySet requestedActionCategories, SnapshotSpan range, CancellationToken cancellationToken) {
        return Task.Factory.StartNew(() => {
                                         var actions = GetOrCreateFixSuggestedActions(range, cancellationToken);
                                         return actions.Any();
                                     },
                                     cancellationToken,
                                     TaskCreationOptions.None, TaskScheduler.Default);
    }

    public IEnumerable<SuggestedActionSet> GetSuggestedActions(ISuggestedActionCategorySet requestedActionCategories, SnapshotSpan range, CancellationToken cancellationToken) {

        var caretPoint = _textView.GetCaretPoint();
        var actions    = GetOrCreateFixSuggestedActions(range, cancellationToken);

        // Nach Katergorie gruppieren
        var actionsByCategory = actions.GroupBy(a => a.Category)
                                       .ToList();

        return actionsByCategory.SelectMany(actionsByCat => BuildSuggestedActionSets(
                                                actionsByCat.Key,
                                                actionsByCat, 
                                                range, 
                                                caretPoint));
    }

    private IEnumerable<SuggestedActionSet> BuildSuggestedActionSets(CodeFixCategory category, IEnumerable<CodeFixSuggestedAction> suggestedActionSets, SnapshotSpan range, SnapshotPoint? caretPoint) {

        // Nach Span Gruppieren
        var groupedActions = suggestedActionSets.GroupBy(action => action.ApplicableToSpan);
        var actionSets     = new List<SuggestedActionSet>();
        foreach (var actionsInSpan in groupedActions) {
            var orderedActions = actionsInSpan.OrderByDescending(codeFixSuggestedAction => codeFixSuggestedAction.Prio);
            actionSets.Add(new SuggestedActionSet(
                               categoryName: ToCategoryName(category),
                               actions: orderedActions,
                               title: "Hi",
                               applicableToSpan: actionsInSpan.Key ?? range));
        }

        // Sortierung nach Nähe zum Caret Point
        var orderedSuggestionSets = actionSets.OrderBy(s => s, new SuggestedActionSetComparer(caretPoint, range));
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

        foreach (var action in actionSet.Actions) {
            if (seenTitles.Add(action.DisplayText)) {
                actions.Add(action);
            }
        }

        return actions.Count == 0
            ? null
            : new SuggestedActionSet(
                categoryName: actionSet.CategoryName,
                actions: actions,
                title: actionSet.Title,
                priority: actionSet.Priority,
                applicableToSpan: actionSet.ApplicableToSpan);
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

    private ImmutableList<CodeFixSuggestedAction> GetOrCreateFixSuggestedActions(SnapshotSpan range, CancellationToken cancellationToken) {

        var cachedActionSets = _cachedSuggestedActionSets;
        ImmutableList<CodeFixSuggestedAction> suggestedActionSets =
            IsCacheValid(cachedActionSets, range) ? cachedActionSets.SuggestedActionSets : BuildSuggestedActions(range, cancellationToken);
        return suggestedActionSets;
    }

    protected ImmutableList<CodeFixSuggestedAction> BuildSuggestedActions(SnapshotSpan range, CancellationToken cancellationToken) {

        var codeGenerationUnitAndSnapshot = SemanticModelService?.CodeGenerationUnitAndSnapshot;
        if (codeGenerationUnitAndSnapshot == null || !codeGenerationUnitAndSnapshot.IsCurrent(range.Snapshot)) {
            _cachedSuggestedActionSets = null;
            return ImmutableList<CodeFixSuggestedAction>.Empty;
        }

        var parameter  = new CodeFixSuggestedActionParameter(range, codeGenerationUnitAndSnapshot, _textView);
        var actionsets = _codeFixSuggestedActionProviderService.GetCodeFixSuggestedActions(parameter, cancellationToken).ToImmutableList();

        if (cancellationToken.IsCancellationRequested || actionsets.Count == 0) {
            return ImmutableList<CodeFixSuggestedAction>.Empty;
        }

        var actionsetsAndRange = new SuggestedActionSetsAndRange(range, actionsets);

        _cachedSuggestedActionSets = actionsetsAndRange;

        return actionsetsAndRange.SuggestedActionSets;
    }

    sealed class SuggestedActionSetsAndRange {

        public SuggestedActionSetsAndRange(SnapshotSpan range, ImmutableList<CodeFixSuggestedAction> suggestedActionSets) {
            Range               = range;
            SuggestedActionSets = suggestedActionSets;
        }

        public SnapshotSpan                          Range               { get; }
        public ImmutableList<CodeFixSuggestedAction> SuggestedActionSets { get; }

    }

}