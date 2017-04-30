#region Using Directives

using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Language.Intellisense;
using Pharmatechnik.Nav.Language.Extension.Images;
using Pharmatechnik.Nav.Language.Extension.LanguageService;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.StatementCompletion {

    class CompletionSource: SemanticModelServiceDependent, ICompletionSource {

        readonly ITextBuffer _buffer;
        bool _disposed;

        public CompletionSource(ITextBuffer buffer):base (buffer) {
            _buffer = buffer;
        }

        public void AugmentCompletionSession(ICompletionSession session, IList<CompletionSet> completionSets) {
            if(_disposed) {
                throw new ObjectDisposedException("OokCompletionSource");
            }

            var codeGenerationUnit = SemanticModelService.CodeGenerationUnitAndSnapshot?.CodeGenerationUnit;
            if(codeGenerationUnit == null) {
                return;
            }

            ITextSnapshot snapshot = _buffer.CurrentSnapshot;
            var snapshotPoint = session.GetTriggerPoint(snapshot);
            if(snapshotPoint == null) {
                return;
            }
            
            var triggerPoint = (SnapshotPoint)snapshotPoint;

            SnapshotPoint start = triggerPoint;
            var line = triggerPoint.GetContainingLine();

            while (start > line.Start && !char.IsWhiteSpace((start - 1).GetChar())) {
                start -= 1;
            }
            var applicableToSpan = new SnapshotSpan(start, triggerPoint);
            var startText=applicableToSpan.GetText();

            var extent = TextExtent.FromBounds(triggerPoint, triggerPoint);
            
            var taskDefinition = codeGenerationUnit.TaskDefinitions
                                                    .FirstOrDefault(td => td.Syntax.Extent.IntersectsWith(extent));

            var img=NavLanguagePackage.GetBitmapSource(ImageMonikers.InitConnectionPoint);
          
            List<Completion> completions = new List<Completion>();
            if (taskDefinition != null) {
                foreach(var node in taskDefinition.NodeDeclarations) {
                    if(String.IsNullOrWhiteSpace(startText) || node.Name.StartsWith(startText)) {
                        
                        completions.Add(new Completion(node.Name, node.Name, "Desc", img, "Bla"));
                    }
                }
            }
            img= NavLanguagePackage.GetBitmapSource(KnownMonikers.IntellisenseKeyword);
            foreach(var keyword in SyntaxFacts.Keywords.OrderBy(n=>n)) {
                completions.Add(new Completion(displayText: keyword, insertionText: keyword, description: $"{keyword} Keyword", iconSource: img, iconAutomationText: "keyword"));
            }
            
            var applicableTo = snapshot.CreateTrackingSpan(applicableToSpan, SpanTrackingMode.EdgeInclusive);

            completionSets.Add(new CompletionSet("All", "All", applicableTo, completions, Enumerable.Empty<Completion>()));

            completions = new List<Completion>();
            img = NavLanguagePackage.GetBitmapSource(KnownMonikers.IntellisenseKeyword);
            foreach (var keyword in SyntaxFacts.Keywords.OrderBy(n => n))
            {
                completions.Add(new Completion(displayText: keyword, insertionText: keyword, description: $"{keyword} Keyword", iconSource: img, iconAutomationText: "keyword"));
            }
            completionSets.Add(new CompletionSet(moniker: "Keyword", displayName: "Keyword", applicableTo: applicableTo, completions: completions, completionBuilders: Enumerable.Empty<Completion>()));

        }

        public override void Dispose() {
            base.Dispose();
            _disposed = true;
        }
    }
}
