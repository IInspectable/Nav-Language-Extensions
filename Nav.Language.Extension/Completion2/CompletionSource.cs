#region Using Directives

using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;

using Pharmatechnik.Nav.Language.Extension.Images;
using Pharmatechnik.Nav.Language.Text;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Completion2 {

    class CompletionSource: SemanticModelServiceDependent, ICompletionSource {

        readonly ITextBuffer _buffer;
        bool                 _disposed;

        public CompletionSource(ITextBuffer buffer): base(buffer) {
            _buffer = buffer;
        }

        public void AugmentCompletionSession(ICompletionSession session, IList<CompletionSet> completionSets) {

            ThreadHelper.ThrowIfNotOnUIThread();

            if (_disposed) {
                throw new ObjectDisposedException("OokCompletionSource");
            }

            var codeGenerationUnit = SemanticModelService.CodeGenerationUnitAndSnapshot?.CodeGenerationUnit;
            if (codeGenerationUnit == null) {
                return;
            }

            ITextSnapshot snapshot      = _buffer.CurrentSnapshot;
            var           snapshotPoint = session.GetTriggerPoint(snapshot);
            if (snapshotPoint == null) {
                return;
            }

            var triggerPoint = (SnapshotPoint) snapshotPoint;

            var start = triggerPoint;
            var line  = triggerPoint.GetContainingLine();

            while (start > line.Start && char.IsLetterOrDigit((start - 1).GetChar()) ) {
                start -= 1;
            }

            bool showEdgeModes=false;
            if (start > line.Start && (start - 1).GetChar() == '-') {

                start -= 1;
                showEdgeModes=true;
            }

            var applicableToSpan = new SnapshotSpan(start, triggerPoint);
            var applicableTo = snapshot.CreateTrackingSpan(applicableToSpan, SpanTrackingMode.EdgeInclusive);
            string moniker     = null;
            var    completions = new List<Completion4>();

            // Edge Modes
            if (showEdgeModes) {
                
                completions.Add(CreateKeywordCompletion(SyntaxFacts.GoToEdgeKeyword));
                completions.Add(CreateKeywordCompletion(SyntaxFacts.ModalEdgeKeyword));

                moniker = "keyword";

                CreateCompletionSet(moniker, completionSets, completions, applicableTo);

                return;
            }
            

            var extent = TextExtent.FromBounds(triggerPoint, triggerPoint);

            var taskDefinition = codeGenerationUnit.TaskDefinitions
                                                   .FirstOrDefault(td => td.Syntax.Extent.IntersectsWith(extent));

            

            if (taskDefinition != null) {

                // Exit Connection Points
                var exitNodeName = "";

                if (start > line.Start && (start - 1).GetChar() == ':') {

                    var exitNodeEnd   = start - 1;
                    var exitNodeStart = exitNodeEnd;
                    while (exitNodeStart > line.Start && !char.IsWhiteSpace((exitNodeStart - 1).GetChar())) {
                        exitNodeStart -= 1;
                    }

                    var exitNodeSpan = new SnapshotSpan(exitNodeStart, exitNodeEnd);
                    exitNodeName = exitNodeSpan.GetText();

                    if (!String.IsNullOrEmpty(exitNodeName)) {

                        var exitNodeCandidate = taskDefinition.NodeDeclarations
                                                              .OfType<ITaskNodeSymbol>()
                                                              .FirstOrDefault(n => n.Name == exitNodeName);

                        if (exitNodeCandidate?.Declaration != null) {
                            foreach (var cp in exitNodeCandidate.Declaration.Exits()) {

                                completions.Add(CreateSymbolCompletion(cp, cp.Name));

                                moniker = "exitNode";
                            }
                        }

                        if (completions.Any()) {
                            CreateCompletionSet(moniker, completionSets, completions, applicableTo);
                            return;
                        }
                    }
                }
                
                // Node Completions
                foreach (var node in taskDefinition.NodeDeclarations.OrderBy(n => n.Name)) {

                    var description = node.Syntax.ToString();

                    completions.Add(CreateSymbolCompletion(node, description));

                    moniker = "keyword";
                }

            }

            if (!completions.Any()) {
                moniker = "keyword";
            }

            // Keywords
            foreach (var keyword in SyntaxFacts.NavKeywords.OrderBy(n => n)) {

                completions.Add(CreateKeywordCompletion(keyword));
            }

            CreateCompletionSet(moniker, completionSets, completions, applicableTo);

        }

        public override void Dispose() {
            base.Dispose();
            _disposed = true;
        }

        private static void CreateCompletionSet(string moniker, IList<CompletionSet> completionSets, List<Completion4> list, ITrackingSpan applicableTo) {
            if (list.Any()) {
                if (moniker == "keyword") {
                    // IntellisenseFilter[] filters = new[] {
                    //     new IntellisenseFilter(KnownMonikers.Property,   "Standard rules (Alt + S)",      "s", "Standard"),
                    //     new IntellisenseFilter(KnownMonikers.CSFileNode, "C# analysis rules (Alt + C)",   "c", "CSharp"),
                    //     new IntellisenseFilter(KnownMonikers.DotNET,     ".NET analysis rules (Alt + D)", "d", "DotNe"),
                    // };

                    completionSets.Add(new FilteredCompletionSet(moniker, applicableTo, list, Enumerable.Empty<Completion4>(), null));
                } else {
                    completionSets.Add(new FilteredCompletionSet(moniker, applicableTo, list, Enumerable.Empty<Completion4>(), null));
                }
            }
        }

        Completion4 CreateSymbolCompletion(ISymbol symbol, string description) {

            var imageMoniker = ImageMonikers.FromSymbol(symbol);

            var completion = new Completion4(displayText: symbol.Name,
                                             insertionText: symbol.Name,
                                             description: description,
                                             iconMoniker: imageMoniker,
                                             iconAutomationText: null,
                                             attributeIcons: null);

            completion.Properties.AddProperty(CompletionElementProvider.SymbolPropertyName, symbol);

            return completion;
        }

        Completion4 CreateKeywordCompletion(string keyword) {

            var completion = new Completion4(displayText: keyword,
                                             insertionText: keyword,
                                             description: $"{keyword} Keyword",
                                             iconMoniker: KnownMonikers.IntellisenseKeyword,
                                             iconAutomationText: null,
                                             attributeIcons: null);

            completion.Properties.AddProperty(CompletionElementProvider.KeywordPropertyName, keyword);

            return completion;
        }

    }

}