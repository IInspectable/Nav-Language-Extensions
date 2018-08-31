#region Using Directives

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Pharmatechnik.Nav.Language.Extension.Common;
using Pharmatechnik.Nav.Language.Text;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Classification {

    sealed class SemanticClassifier : SemanticModelServiceDependent, IClassifier {

        SemanticClassifier(IClassificationTypeRegistryService classificationTypeRegistryService, ITextBuffer textBuffer) : base(textBuffer) {
            ClassificationTypeRegistryService = classificationTypeRegistryService;
        }

        public static IClassifier GetOrCreateSingelton(IClassificationTypeRegistryService classificationTypeRegistryService, ITextBuffer textBuffer) {
            return new TextBufferScopedClassifier(
                textBuffer,
                typeof(SemanticClassifier),
                () =>
                    new SemanticClassifier(classificationTypeRegistryService, textBuffer));
        }
        
        public IClassificationTypeRegistryService ClassificationTypeRegistryService { get; }

        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span) {
            var result = new List<ClassificationSpan>();

            var codeGenerationUnitAndSnapshot = SemanticModelService.CodeGenerationUnitAndSnapshot;
            if(codeGenerationUnitAndSnapshot == null) {
                return result;
            }

            // Dead Code
            result.AddRange(
                BuildClassificationSpan(
                    textExtents                  : GetDeadCodeExtents(codeGenerationUnitAndSnapshot.CodeGenerationUnit),
                    classificationType           : ClassificationTypeRegistryService.GetClassificationType(ClassificationTypeNames.DeadCode),
                    range                        : span,
                    codeGenerationUnitAndSnapshot: codeGenerationUnitAndSnapshot));

            // Semantic Highlighting
            var advancedOptions = NavLanguagePackage.AdvancedOptions;
            if (advancedOptions.SemanticHighlighting) {

                // Init Nodes
                result.AddRange(
                    BuildClassificationSpan(
                        textExtents                  : GetInitNodeExtents(codeGenerationUnitAndSnapshot.CodeGenerationUnit),
                        classificationType           : ClassificationTypeRegistryService.GetClassificationType(ClassificationTypeNames.ConnectionPoint),
                        range                        : span,
                        codeGenerationUnitAndSnapshot: codeGenerationUnitAndSnapshot));

                // Exit Nodes
                result.AddRange(
                    BuildClassificationSpan(
                        textExtents                  : GetExitNodeExtents(codeGenerationUnitAndSnapshot.CodeGenerationUnit),
                        classificationType           : ClassificationTypeRegistryService.GetClassificationType(ClassificationTypeNames.ConnectionPoint),
                        range                        : span,
                        codeGenerationUnitAndSnapshot: codeGenerationUnitAndSnapshot));

                // Choice Nodes
                result.AddRange(
                    BuildClassificationSpan(
                        textExtents                  : GetChoiceNodeExtents(codeGenerationUnitAndSnapshot.CodeGenerationUnit),
                        classificationType           : ClassificationTypeRegistryService.GetClassificationType(ClassificationTypeNames.ChoiceNode),
                        range                        : span,
                        codeGenerationUnitAndSnapshot: codeGenerationUnitAndSnapshot));
            }
            return result.OrderBy(c => c.Span.Start).ToList();
        }

        static IEnumerable<ClassificationSpan> BuildClassificationSpan(IEnumerable<TextExtent> textExtents, IClassificationType classificationType, SnapshotSpan range, CodeGenerationUnitAndSnapshot codeGenerationUnitAndSnapshot) {

            var rangeExtent = TextExtent.FromBounds(range.Start.Position, range.End.Position);
            foreach (var extent in textExtents.Where(e=> !e.IsMissing)) {

                if (!extent.IntersectsWith(rangeExtent)) {
                    continue;
                }

                var sourceSpan = new SnapshotSpan(codeGenerationUnitAndSnapshot.Snapshot, new Span(extent.Start, extent.Length));

                var classification = new ClassificationSpan(
                    sourceSpan.TranslateTo(range.Snapshot, SpanTrackingMode.EdgeExclusive),
                    classificationType);

                yield return classification;
            }
        }

        static IEnumerable<TextExtent> GetDeadCodeExtents(CodeGenerationUnit codeGenerationUnit) {
            var diagnostics = codeGenerationUnit.Diagnostics;
            var candidates  = diagnostics.Where(diagnostic => diagnostic.Category == DiagnosticCategory.DeadCode);
            return candidates.SelectMany(diag => diag.GetLocations()).Select(loc => loc.Extent);
        }

        static IEnumerable<TextExtent> GetChoiceNodeExtents(CodeGenerationUnit codeGenerationUnit) {
            var choiceNodes = codeGenerationUnit.Symbols.OfType<IChoiceNodeSymbol>();
            foreach(var choiceNode in choiceNodes) {
                yield return choiceNode.Syntax.Identifier.Extent;

                foreach(var sourceNode in choiceNode.Outgoings.Select(trans => trans.SourceReference).Where(source => source != null)) {
                    yield return sourceNode.Location.Extent;
                }
                foreach(var targetNode in choiceNode.Incomings.Select(trans => trans.TargetReference).Where(target => target != null)) {
                    yield return targetNode.Location.Extent;
                }
            }
        }

        static IEnumerable<TextExtent> GetInitNodeExtents(CodeGenerationUnit codeGenerationUnit) {
            var initNodes = codeGenerationUnit.Symbols.OfType<IInitNodeSymbol>();
            foreach (var initNode in initNodes) {
                if(initNode.Alias != null) {
                    yield return initNode.Alias.Location.Extent;
                }                
                foreach (var sourceNode in initNode.Outgoings.Select(trans => trans.SourceReference).Where(source => source != null)) {
                    yield return sourceNode.Location.Extent;
                }               
            }
        }

        static IEnumerable<TextExtent> GetExitNodeExtents(CodeGenerationUnit codeGenerationUnit) {
            var exitNodes = codeGenerationUnit.Symbols.OfType<IExitNodeSymbol>();
            foreach (var exitNode in exitNodes) {
                yield return exitNode.Syntax.Identifier.Extent;
               
                foreach (var targetNode in exitNode.Incomings.Select(trans => trans.TargetReference).Where(target => target != null)) {
                    yield return targetNode.Location.Extent;
                }
            }
        }        

        protected override void OnSemanticModelChanged(object sender, SnapshotSpanEventArgs e) {
            ClassificationChanged?.Invoke(this, new ClassificationChangedEventArgs(e.Span));
        }
        
        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;
    }
}