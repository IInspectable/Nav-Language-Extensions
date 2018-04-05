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

            var start = triggerPoint;
            var line = triggerPoint.GetContainingLine();

            while (start > line.Start && char.IsLetterOrDigit((start - 1).GetChar()) && (start - 1).GetChar()!=':' ) {
                start -= 1;
            }
            var applicableToSpan = new SnapshotSpan(start, triggerPoint);
            
            var exitNodeName = "";

            if (start > line.Start && (start - 1).GetChar() == ':') {

                var exitNodeEnd   = start - 1;
                var exitNodeStart = exitNodeEnd;
                while (exitNodeStart > line.Start && !char.IsWhiteSpace((exitNodeStart - 1).GetChar())) {
                    exitNodeStart -= 1;
                }

                var exitNodeSpan = new SnapshotSpan(exitNodeStart, exitNodeEnd);
                exitNodeName = exitNodeSpan.GetText();
            }

            var applicableTo = snapshot.CreateTrackingSpan(applicableToSpan, SpanTrackingMode.EdgeInclusive);

            List<Completion> completions = new List<Completion>();
            
            var extent = TextExtent.FromBounds(triggerPoint, triggerPoint);
            
            var taskDefinition = codeGenerationUnit.TaskDefinitions
                                                    .FirstOrDefault(td => td.Syntax.Extent.IntersectsWith(extent));
          
            if (taskDefinition != null) {

                var cpsAdded = false;
                if(!String.IsNullOrEmpty(exitNodeName)) {

                    var exitNodeCandidate=taskDefinition.NodeDeclarations
                                                        .OfType<ITaskNodeSymbol>()
                                                        .FirstOrDefault(n => n.Name == exitNodeName);
                    
                    if (exitNodeCandidate?.Declaration != null) {
                        foreach (var cp in exitNodeCandidate.Declaration.Exits()) {
                            var imageMoniker = ImageMonikers.FromSymbol(cp);
                            var imgSrc       =NavLanguagePackage.GetBitmapSource(imageMoniker);
                            var desc         = "Foo";
                    
                            completions.Add(new Completion(cp.Name, cp.Name, desc, imgSrc, "Bla"));
                            cpsAdded=true;
                        }
                    }
                }

                if (cpsAdded) {
                    completionSets.Add(new CompletionSet("All", "All", applicableTo, completions, Enumerable.Empty<Completion>()));
                    return;

                }

                foreach (var node in taskDefinition.NodeDeclarations) {

                    var imageMoniker = ImageMonikers.FromSymbol(node);
                    var imgSrc       = NavLanguagePackage.GetBitmapSource(imageMoniker);
                    var desc         = node.Syntax.ToString();

                    completions.Add(new Completion(node.Name, node.Name, desc, imgSrc, "Bla"));
                }
                
            }

            var img= NavLanguagePackage.GetBitmapSource(KnownMonikers.IntellisenseKeyword);
            foreach(var keyword in SyntaxFacts.Keywords.OrderBy(n=>n)) {
                completions.Add(new Completion(displayText: keyword, insertionText: keyword, description: $"{keyword} Keyword", iconSource: img, iconAutomationText: "keyword"));
            }
            
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
