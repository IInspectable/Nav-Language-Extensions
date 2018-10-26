#region Using Directives

using System;
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
            
            // Text
            var textExtent = referenceLine.ExtentWithoutLineEndings;

            var textParts = reference.SyntaxTree
                                     .GetClassifiedText(textExtent)
                                     .ToImmutableArray();

          
            var textHighlightExtent = new TextExtent(start:  reference.Start - referenceLine.Start,
                                                     length: reference.Location.Length);

            // ToolTip
            var tipExtent = GetToolTipExtent(referenceLine);
            var toolTipParts = reference.SyntaxTree
                                        .GetClassifiedText(tipExtent)
                                        .ToImmutableArray();

            var toolTipHighlightExtent = new TextExtent(start : reference.Start - tipExtent.Start,
                                                        length: reference.Location.Length);

            var referenceItem = new ReferenceItem(definition            : definitionItem,
                                                  location              : reference.Location,
                                                  textParts             : textParts,
                                                  textHighlightExtent   : textHighlightExtent,
                                                  toolTipParts          : toolTipParts,
                                                  toolTipHighlightExtent: toolTipHighlightExtent);
            return referenceItem;
        }

        private const int  ToolTipLinesOnOneSide = 3;

        private static TextExtent GetToolTipExtent(SourceTextLine referenceLine) {

            var sourceText = referenceLine.SourceText;
            if (sourceText.TextLines.Count <= 1) {
                return referenceLine.ExtentWithoutLineEndings;
            }

            var lineNumber = referenceLine.Line;

            var firstLine = sourceText.TextLines[Math.Max(lineNumber - ToolTipLinesOnOneSide, 0)];
            var lastLine  = sourceText.TextLines[Math.Min(lineNumber + ToolTipLinesOnOneSide, sourceText.TextLines.Count - 1)];

            return TextExtent.FromBounds(firstLine.Start, lastLine.ExtentWithoutLineEndings.End);

        }

    }

}