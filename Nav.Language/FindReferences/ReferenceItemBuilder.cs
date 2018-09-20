#region Using Directives

using System;
using System.Linq;
using System.Collections.Immutable;

using JetBrains.Annotations;

using Pharmatechnik.Nav.Language.Text;

#endregion

namespace Pharmatechnik.Nav.Language.FindReferences {

    class ReferenceItemBuilder {

        [CanBeNull]
        public static ReferenceItem Invoke(DefinitionItem definitionItem, ISymbol reference) {

            if (definitionItem == null || reference.SyntaxTree == null) {
                return null;
            }

            var referenceLine = reference.SyntaxTree.SourceText.GetTextLineAtPosition(reference.Location.Start);

            // Prefix
            var prefixParts = PrefixPartsBuilder.Invoke(reference);

            // Text
            var textExtent = referenceLine.ExtentWithoutLineEndings;

            var textParts = reference.SyntaxTree
                                     .GetClassifiedText(textExtent)
                                     .ToImmutableArray();

            if (prefixParts.Any()) {
                textParts = prefixParts.AddRange(textParts);

            }

            var textHighlightExtent = new TextExtent(start: prefixParts.Length() + reference.Start - referenceLine.Start,
                                                     length: reference.Location.Length);

            // Preview
            var previewExtent = GetPreviewExtent(referenceLine);
            var previewParts  = reference.SyntaxTree
                                        .GetClassifiedText(previewExtent)
                                        .ToImmutableArray();

            var previewHighlightExtent = new TextExtent(start : reference.Start - previewExtent.Start,
                                                        length: reference.Location.Length);

            var referenceItem = new ReferenceItem(definition            : definitionItem,
                                                  location              : reference.Location,
                                                  textParts             : textParts,
                                                  textHighlightExtent   : textHighlightExtent,
                                                  previewParts          : previewParts,
                                                  previewHighlightExtent: previewHighlightExtent);
            return referenceItem;
        }

        private const int  PreviewLinesOnOneSide = 3;

        private static TextExtent GetPreviewExtent(SourceTextLine referenceLine) {

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