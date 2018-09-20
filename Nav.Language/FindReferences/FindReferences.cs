#region Using Directives

using System;
using System.Collections.Immutable;
using System.Threading.Tasks;

using JetBrains.Annotations;

using Pharmatechnik.Nav.Language.Text;

#endregion

namespace Pharmatechnik.Nav.Language.FindReferences {

    public class ReferenceFinder {

        public static Task FindReferences(FindReferencesArgs args) {

            if (args == null) {
                throw new ArgumentNullException(nameof(args));
            }

            return Task.Run(async () => {

                var definition = FindDefinitionVisitor.Invoke(args.Symbol);
                if (definition == null) {
                    return;
                }

                var searchRoot     = args.SearchDirectory?.FullName ?? definition.SyntaxTree?.SourceText.FileInfo?.FullName ?? "";
                var definitionItem = new DefinitionItem(searchRoot, definition, definition.ToDisplayParts());

                foreach (var reference in FindReferencesVisitor.Invoke(args, definitionItem)
                    //.OrderBy(d => d.Location.StartLine)
                    //.ThenBy(d => d.Location.StartCharacter)
                ) {

                    if (args.Context.CancellationToken.IsCancellationRequested) {
                        return;
                    }

                    var referenceItem = ToReferenceItem(definitionItem, reference);

                    if (referenceItem == null) {
                        continue;
                    }

                    await args.Context.OnReferenceFoundAsync(referenceItem);

                }

            });

        }

        [CanBeNull]
        static ReferenceItem ToReferenceItem(DefinitionItem definitionItem, ISymbol reference) {

            if (definitionItem == null || reference.SyntaxTree == null) {
                return null;
            }

            var referenceLine = reference.SyntaxTree.SourceText.GetTextLineAtPosition(reference.Location.Start);

            // Text
            var textExtent = referenceLine.ExtentWithoutLineEndings;

            var prefix               = "";
            var prefixClassification = TextClassification.Text;
            if (reference is INodeSymbol node) {
                prefix               = $"{node.ContainingTask.Name} \u220B";
                prefixClassification = TextClassification.TaskName;
            }

            if (reference is INodeReferenceSymbol nodeReference && nodeReference.Declaration is INodeSymbol nodeSymbol) {
                prefix               = $"{nodeSymbol.ContainingTask.Name} \u220B";
                prefixClassification = TextClassification.TaskName;
            }

            var textParts = reference.SyntaxTree
                                     .GetClassifiedText(textExtent)
                                     .ToImmutableArray();

            if (!String.IsNullOrEmpty(prefix)) {
                textParts = textParts.Insert(0, new ClassifiedText(prefix, prefixClassification));

            }

            var textHighlightExtent = new TextExtent(start: reference.Start - referenceLine.Start + prefix.Length,
                                                     length: reference.Location.Length);

            // Preview
            var previewExtent = GetPreviewExtent(referenceLine);
            var previewParts = reference.SyntaxTree
                                        .GetClassifiedText(previewExtent)
                                        .ToImmutableArray();

            var previewHighlightExtent = new TextExtent(start: reference.Start - previewExtent.Start,
                                                        length: reference.Location.Length);

            var referenceItem = new ReferenceItem(definition: definitionItem,
                                                  location: reference.Location,
                                                  textParts: textParts,
                                                  textHighlightExtent: textHighlightExtent,
                                                  previewParts: previewParts,
                                                  previewHighlightExtent: previewHighlightExtent);
            return referenceItem;
        }

        const int PreviewLinesOnOneSide = 3;

        static TextExtent GetPreviewExtent(SourceTextLine referenceLine) {

            var sourceText = referenceLine.SourceText;
            if (sourceText.TextLines.Count <= 1) {
                return referenceLine.ExtentWithoutLineEndings;
            }

            var lineNumber = referenceLine.Line;

            var firstLine = sourceText.TextLines[Math.Max(lineNumber - PreviewLinesOnOneSide, 0)];
            var lastLine  = sourceText.TextLines[Math.Min(lineNumber + PreviewLinesOnOneSide, sourceText.TextLines.Count - 1)];

            return TextExtent.FromBounds(firstLine.Start, lastLine.ExtentWithoutLineEndings.End);

        }

    }

}