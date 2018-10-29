#region Using Directives

using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;

using Pharmatechnik.Nav.Language.Extension.Common;
using Pharmatechnik.Nav.Language.Text;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Classification {

    sealed class SemanticClassificationTagger: SemanticModelServiceDependent, ITagger<IClassificationTag> {

        SemanticClassificationTagger(IClassificationTypeRegistryService classificationTypeRegistryService, ITextBuffer textBuffer): base(textBuffer) {
            ClassificationTypeRegistryService = classificationTypeRegistryService;
        }

        public static SemanticClassificationTagger GetOrCreateSingelton(IClassificationTypeRegistryService classificationTypeRegistryService, ITextBuffer textBuffer) {
            return TextBufferScopedValue<SemanticClassificationTagger>.GetOrCreate(textBuffer, typeof(SemanticClassificationTagger), () => new SemanticClassificationTagger(classificationTypeRegistryService, textBuffer))
                                                                      .Value;
        }

        public IClassificationTypeRegistryService ClassificationTypeRegistryService { get; }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public IEnumerable<ITagSpan<IClassificationTag>> GetTags(NormalizedSnapshotSpanCollection spans) {

            var codeGenerationUnitAndSnapshot = SemanticModelService.CodeGenerationUnitAndSnapshot;
            if (codeGenerationUnitAndSnapshot == null) {
                yield break;
            }

            foreach (var span in spans) {

                // Dead Code
                foreach (var deadCode in BuildClassificationSpan(
                        textExtents                  : GetDeadCodeExtents(codeGenerationUnitAndSnapshot.CodeGenerationUnit),
                        classificationType           : ClassificationTypeRegistryService.GetClassificationType(ClassificationTypeNames.DeadCode),
                        range                        : span,
                        codeGenerationUnitAndSnapshot: codeGenerationUnitAndSnapshot)) {

                    yield return deadCode;
                }

                // Semantic Highlighting
                var advancedOptions = NavLanguagePackage.AdvancedOptions;
                if (advancedOptions.SemanticHighlighting) {

                    // Init Nodes
                    foreach (var initNode in BuildClassificationSpan(
                            textExtents                  : GetInitNodeExtents(codeGenerationUnitAndSnapshot.CodeGenerationUnit),
                            classificationType           : ClassificationTypeRegistryService.GetClassificationType(ClassificationTypeNames.ConnectionPoint),
                            range                        : span,
                            codeGenerationUnitAndSnapshot: codeGenerationUnitAndSnapshot)) {

                        yield return initNode;
                    }

                    // Exit Nodes
                    foreach (var exitNode in BuildClassificationSpan(
                            textExtents                  : GetExitNodeExtents(codeGenerationUnitAndSnapshot.CodeGenerationUnit),
                            classificationType           : ClassificationTypeRegistryService.GetClassificationType(ClassificationTypeNames.ConnectionPoint),
                            range                        : span,
                            codeGenerationUnitAndSnapshot: codeGenerationUnitAndSnapshot)) {

                        yield return exitNode;
                    }

                    // Choice Nodes
                    foreach (var choiceNode in BuildClassificationSpan(
                            textExtents                  : GetChoiceNodeExtents(codeGenerationUnitAndSnapshot.CodeGenerationUnit),
                            classificationType           : ClassificationTypeRegistryService.GetClassificationType(ClassificationTypeNames.ChoiceNode),
                            range                        : span,
                            codeGenerationUnitAndSnapshot: codeGenerationUnitAndSnapshot)) {

                        yield return choiceNode;
                    }
                }
            }
        }

        protected override void OnSemanticModelChanged(object sender, SnapshotSpanEventArgs e) {
            TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(e.Span));
        }

        static IEnumerable<ITagSpan<IClassificationTag>> BuildClassificationSpan(IEnumerable<TextExtent> textExtents, IClassificationType classificationType, SnapshotSpan range, CodeGenerationUnitAndSnapshot codeGenerationUnitAndSnapshot) {

            var rangeExtent = TextExtent.FromBounds(range.Start.Position, range.End.Position);

            foreach (var extent in textExtents.Where(e => !e.IsMissing)) {

                if (!extent.IntersectsWith(rangeExtent)) {
                    continue;
                }

                var tokenSpan = new SnapshotSpan(codeGenerationUnitAndSnapshot.Snapshot, new Span(extent.Start, extent.Length));
                var tagSpan   = tokenSpan.TranslateTo(range.Snapshot, SpanTrackingMode.EdgeExclusive);
                var tag       = new ClassificationTag(classificationType);

                yield return new TagSpan<IClassificationTag>(tagSpan, tag);

            }
        }

        static IEnumerable<TextExtent> GetDeadCodeExtents(CodeGenerationUnit codeGenerationUnit) {
            var diagnostics = codeGenerationUnit.Diagnostics;
            var candidates  = diagnostics.Where(diagnostic => diagnostic.Category == DiagnosticCategory.DeadCode);
            return candidates.SelectMany(diag => diag.GetLocations()).Select(loc => loc.Extent);
        }

        static IEnumerable<TextExtent> GetChoiceNodeExtents(CodeGenerationUnit codeGenerationUnit) {
            var choiceNodes = codeGenerationUnit.Symbols.OfType<IChoiceNodeSymbol>();
            foreach (var choiceNode in choiceNodes) {
                yield return choiceNode.Syntax.Identifier.Extent;

                foreach (var sourceNode in choiceNode.Outgoings.Select(trans => trans.SourceReference).Where(source => source != null)) {
                    yield return sourceNode.Location.Extent;
                }

                foreach (var targetNode in choiceNode.Incomings.Select(trans => trans.TargetReference).Where(target => target != null)) {
                    yield return targetNode.Location.Extent;
                }
            }
        }

        static IEnumerable<TextExtent> GetInitNodeExtents(CodeGenerationUnit codeGenerationUnit) {
            var initNodes = codeGenerationUnit.Symbols.OfType<IInitNodeSymbol>();
            foreach (var initNode in initNodes) {
                if (initNode.Alias != null) {
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

    }

}