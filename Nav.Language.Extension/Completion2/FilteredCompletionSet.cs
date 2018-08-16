#region Using Directives

using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.PatternMatching;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Completion2 {

    public class FilteredCompletionSet: CompletionSet2 {

        readonly IPatternMatcherFactory                   _patternMatcherFactory;
        readonly FilteredObservableCollection<Completion> _currentCompletions;
        readonly List<IIntellisenseFilter>                _activeFilters;

        readonly ITrackingSpan _typed;
        string                 _typedText;

        public FilteredCompletionSet(string moniker,
                                     ITrackingSpan typed,
                                     ITrackingSpan applicableTo,
                                     IList<Completion4> completions,
                                     IEnumerable<Completion> completionBuilders,
                                     IReadOnlyList<IIntellisenseFilter> filters,
                                     IPatternMatcherFactory patternMatcherFactory)
            : base(moniker, "All", applicableTo, completions, completionBuilders, filters) {
            _patternMatcherFactory = patternMatcherFactory;

            _typed = typed ?? applicableTo;

            var observableCollection = new BulkObservableCollection<Completion>();
            observableCollection.AddRange(completions);

            _currentCompletions = new FilteredObservableCollection<Completion>(observableCollection);

            _activeFilters = new List<IIntellisenseFilter>();

        }

        public override IList<Completion> Completions => _currentCompletions;

        public override void Filter() {
            // This is handled in SelectBestMatch
        }

        public override void SelectBestMatch() {

            _typedText = _typed.GetText(_typed.TextBuffer.CurrentSnapshot);

            CustomFilter();

            bool       isCompletionUnique = false;
            Completion completionToSelect = null;
            if (!String.IsNullOrEmpty(_typedText)) {

                var orderedByMatch = _currentCompletions.OrderByDescending(c => GetHighlightedSpansInDisplayText(c.DisplayText).Sum(s => s.Length))
                                                        .ToList();

                if (orderedByMatch.Any()) {
                    isCompletionUnique = orderedByMatch.Count == 1;
                    completionToSelect = orderedByMatch.First();
                }
            }

            if (completionToSelect != null) {
                SelectionStatus = new CompletionSelectionStatus(completionToSelect, isCompletionUnique, isCompletionUnique);
            } else {
                SelectBestMatch(CompletionMatchType.MatchDisplayText, false);
            }
        }

        private void CustomFilter() {

            IReadOnlyList<IIntellisenseFilter> currentActiveFilters = Filters;

            if (currentActiveFilters != null && currentActiveFilters.Count > 0) {

                var activeFilters = currentActiveFilters.Where(f => f.IsChecked).ToList();

                if (!activeFilters.Any()) {
                    activeFilters = currentActiveFilters.ToList();
                }

                _activeFilters.Clear();
                _activeFilters.AddRange(activeFilters);

                _currentCompletions.Filter(DoesCompletionMatchAutomationText);

            } else {

                if (!CompletionController.ShowAllMembers) {
                    _currentCompletions.Filter(DoesCompletionMatchDisplayText);
                }

            }
        }

        private bool DoesCompletionMatchDisplayText(Completion completion) {
            return _typedText.Length == 0 ||
                   GetMatchedParts(completion.DisplayText, _typedText).Any();
        }

        private bool DoesCompletionMatchAutomationText(Completion completion) {

            bool matchesFilter = _activeFilters.Exists(filter => String.Equals(filter.AutomationText, completion.IconAutomationText, StringComparison.OrdinalIgnoreCase));

            return matchesFilter &&
                   (CompletionController.ShowAllMembers                        ||
                    _typedText.Length                                         == 0 ||
                    GetMatchedParts(completion.DisplayText, _typedText).Count > 0);
        }

        public override IReadOnlyList<Span> GetHighlightedSpansInDisplayText(string displayText) {
            return GetHighlightedSpans(displayText, _typedText);
        }

        List<Span> GetHighlightedSpans(string text, string typed) {

            return GetMatchedParts(text, typed);

        }

        List<Span> GetMatchedParts(string text, string typed) {

            if (!String.IsNullOrEmpty(typed)) {

                var patternMatcher = _patternMatcherFactory.CreatePatternMatcher(typed, new PatternMatcherCreationOptions(System.Globalization.CultureInfo.CurrentCulture, PatternMatcherCreationFlags.IncludeMatchedSpans));

                var match = patternMatcher.TryMatch(text);

                if (match.HasValue && match.Value.MatchedSpans.Any()) {
                    return match.Value.MatchedSpans.ToList();
                }
            }

            return Enumerable.Empty<Span>().ToList();

        }

    }

}