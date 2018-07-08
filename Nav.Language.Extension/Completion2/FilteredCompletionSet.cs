#region Using Directives

using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;

using Pharmatechnik.Nav.Language.Extension.Common;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Completion2 {

    public class FilteredCompletionSet: CompletionSet2 {

        static List<Span> DefaultEmptyList => new List<Span>();

        readonly FilteredObservableCollection<Completion> _currentCompletions;
        readonly List<string>                             _activeFilters;

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

            _activeFilters = new List<string>();

        }

        public override IList<Completion> Completions => _currentCompletions;

        public override void Filter() {
            // This is handled in SelectBestMatch
        }

        public override void SelectBestMatch() {

            _typed = ApplicableTo.GetText(ApplicableTo.TextBuffer.CurrentSnapshot);

            CustomFilter();

            var ordered = _currentCompletions.OrderByDescending(c => GetHighlightedSpansInDisplayText(c.DisplayText).Sum(s => s.Length))
                                             .ThenBy(c => c.DisplayText.Length)
                                             .ToList();

            if (ordered.Any()) {
                int count = ordered.Count();
                SelectionStatus = new CompletionSelectionStatus(ordered.First(), count == 1, count == 1);
            } else {
                SelectBestMatch(CompletionMatchType.MatchDisplayText, false);
            }
        }

        private void CustomFilter() {

            IReadOnlyList<IIntellisenseFilter> currentActiveFilters = Filters;

            if (currentActiveFilters != null && currentActiveFilters.Count > 0) {

                var activeFilters = currentActiveFilters.Where(f => f.IsChecked).Select(f => f.AutomationText)
                                                        .ToList();

                if (!activeFilters.Any()) {
                    activeFilters = currentActiveFilters.Select(f => f.AutomationText)
                                                        .ToList();
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
                   GetHighlightedSpansInDisplayText(completion.DisplayText).Any();

            //    completion.DisplayText.IndexOf(_typed, StringComparison.OrdinalIgnoreCase) > -1;
        }

        private bool DoesCompletionMatchAutomationText(Completion completion) {
            return _activeFilters.Exists(x => x.Is(completion.IconAutomationText)) &&
                   (CompletionController.ShowAllMembers                                 ||
                    _typed.Length                                                  == 0 ||
                    GetHighlightedSpansInDisplayText(completion.DisplayText).Count > 0);
        }

        public override IReadOnlyList<Span> GetHighlightedSpansInDisplayText(string displayText) {
            return GetHighlightedSpans(displayText, _typed);
        }

        public static List<Span> GetHighlightedSpans(string text, string typed) {
            var result = GetHighlightedSpansImpl(text, typed);

            //var parts     = typed.CamelHumpSplit();
            //var partSpans = new List<Span>();
            //foreach (var part in parts) {
            //    var spans = GetHighlightedSpansImpl(text, part);
            //    if (!spans.Any()) {
            //        return result;
            //    }

            //    partSpans.AddRange(spans);
            //}

            //result.AddRange(partSpans);

            return result;
        }

        public static List<Span> GetHighlightedSpansImpl(string text, string typed) {

            var textLower  = text.ToLowerInvariant();
            var typedLower = typed.ToLowerInvariant();
            var matches    = new SortedList<int, Span>();
            var match      = string.Empty;

            int startIndex = 0;

            for (int i = 0; i < typedLower.Length; i++) {
                char c = typedLower[i];

                if (!textLower.Contains(match + c)) {

                    if (!matches.Any()) {
                        return DefaultEmptyList;
                    }

                    match      = string.Empty;
                    startIndex = matches.Last().Value.End;
                }

                string current = match + c;
                int    index   = textLower.IndexOf(current, startIndex, StringComparison.Ordinal);

                if (index == -1)
                    return DefaultEmptyList;

                if (index > -1) {
                    matches[index] =  new Span(index, current.Length);
                    match          += c;
                } else {
                    return DefaultEmptyList;
                }
            }

            return matches.Values.ToList();
        }

    }

}