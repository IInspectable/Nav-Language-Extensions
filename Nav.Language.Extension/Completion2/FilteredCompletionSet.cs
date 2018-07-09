#region Using Directives

using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;

using Pharmatechnik.Nav.Language.Extension.Common;
using Pharmatechnik.Nav.Language.Text;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Completion2 {

    public class FilteredCompletionSet: CompletionSet2 {

        readonly FilteredObservableCollection<Completion> _currentCompletions;
        readonly List<IIntellisenseFilter>                _activeFilters;

        string _typed;

        public FilteredCompletionSet(string moniker,
                                     ITrackingSpan applicableTo,
                                     IList<Completion4> completions,
                                     IEnumerable<Completion> completionBuilders,
                                     IReadOnlyList<IIntellisenseFilter> filters)
            : base(moniker, "All", applicableTo, completions, completionBuilders, filters) {

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

            _typed = ApplicableTo.GetText(ApplicableTo.TextBuffer.CurrentSnapshot);

            CustomFilter();

            bool       isCompletionUnique = false;
            Completion completionToSelect = null;
            if (!String.IsNullOrEmpty(_typed)) {

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
            return _typed.Length == 0 ||
                   GetMatchedParts(completion.DisplayText, _typed).Any();
        }

        private bool DoesCompletionMatchAutomationText(Completion completion) {

            bool matchesFilter = _activeFilters.Exists(filter => String.Equals(filter.AutomationText, completion.IconAutomationText, StringComparison.OrdinalIgnoreCase));

            return matchesFilter &&
                   (CompletionController.ShowAllMembers                        ||
                    _typed.Length                                         == 0 ||
                    GetMatchedParts(completion.DisplayText, _typed).Count > 0);
        }

        public override IReadOnlyList<Span> GetHighlightedSpansInDisplayText(string displayText) {
            return GetHighlightedSpans(displayText, _typed);
        }

        static List<Span> GetHighlightedSpans(string text, string typed) {

            return GetMatchedParts(text, typed)
                  .Select(part => part.ToSpan())
                  .ToList();

        }

        static List<TextExtent> GetMatchedParts(string text, string typed) {

            return PatternMatcher.Default
                                 .GetMatchedParts(text, typed);

        }

    }

}