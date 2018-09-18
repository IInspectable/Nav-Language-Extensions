#region Using Directives

using System;
using System.Collections.Immutable;
using System.Threading.Tasks;
using System.Linq;

using JetBrains.Annotations;

using Pharmatechnik.Nav.Language.Text;

#endregion

namespace Pharmatechnik.Nav.Language.FindReferences {

    public class ReferenceFinder {

        public static Task FindReferences(FindReferencesArgs args, IFindReferencesContext context) {

            return Task.Run(async () => {

                var definition = FindDefinitionVisitor.Invoke(args.Symbol);
                if (definition == null) {
                    return;
                }

                var definitionItem = new DefinitionItem(definition, definition.ToDisplayParts());

                foreach (var reference in FindReferencesVisitor.Invoke(definitionItem)
                                                               .OrderBy(d => d.Location.StartLine)
                                                               .ThenBy(d => d.Location.StartCharacter)) {

                    if (context.CancellationToken.IsCancellationRequested) {
                        return;
                    }

                    var referenceItem = ToReferenceItem(definitionItem, reference);

                    if (referenceItem == null) {
                        continue;
                    }

                    await context.OnReferenceFoundAsync(referenceItem);

                }

            });

        }

        [CanBeNull]
        static ReferenceItem ToReferenceItem(DefinitionItem definitionItem, ISymbol reference) {

            if (definitionItem == null || reference.SyntaxTree == null) {
                return null;
            }

            var line = reference.SyntaxTree.SourceText.GetTextLineAtPosition(reference.Location.Start);

            var lineExtent = line.ExtentWithoutLineEndings;

            var lineParts = reference.SyntaxTree
                                     .GetClassifiedText(lineExtent)
                                     .ToImmutableArray();

            var lineHighlightExtent = new TextExtent(start : reference.Start - line.Start,
                                                     length: reference.Location.Length);

            var previewExtent = GetPreviewExtent(line);

            var previewParts = reference.SyntaxTree
                                        .GetClassifiedText(previewExtent)
                                        .ToImmutableArray();

            var previewHighlightExtent = new TextExtent(start : reference.Start - previewExtent.Start,
                                                        length: reference.Location.Length);

            var referenceItem = new ReferenceItem(definition            : definitionItem,
                                                  location              : reference.Location,
                                                  lineParts             : lineParts,
                                                  lineHighlightExtent   : lineHighlightExtent,
                                                  previewParts          : previewParts,
                                                  previewHighlightExtent: previewHighlightExtent);
            return referenceItem;
        }

        const int PreviewLinesOnOneSide = 3;

        static TextExtent GetPreviewExtent(SourceTextLine line) {

            var sourceText = line.SourceText;
            if (sourceText.TextLines.Count <= 1) {
                return line.ExtentWithoutLineEndings;
            }

            var lineNumber = line.Line;

            var firstLine = sourceText.TextLines[Math.Max(lineNumber - PreviewLinesOnOneSide, 0)];
            var lastLine  = sourceText.TextLines[Math.Min(lineNumber + PreviewLinesOnOneSide, sourceText.TextLines.Count - 1)];

            return TextExtent.FromBounds(firstLine.Start, lastLine.ExtentWithoutLineEndings.End);

        }

    }

}