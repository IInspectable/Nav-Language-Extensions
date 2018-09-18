#region Using Directives

using System;
using System.Collections.Immutable;
using System.Threading.Tasks;
using System.Linq;

using Pharmatechnik.Nav.Language.Text;

#endregion

namespace Pharmatechnik.Nav.Language.FindReferences {

    public class ReferenceFinder {

        public static Task FindReferences(ISymbol symbol, IFindReferencesContext context) {

            if (symbol == null) {
                return Task.CompletedTask;
            }

            return Task.Run(async () => {

                var definition = FindDefinitionVisitor.Invoke(symbol);
                if (definition == null) {
                    return;
                }

                var definitionEntry = new DefinitionEntry(definition, definition.ToDisplayParts());

                foreach (var reference in FindReferencesVisitor.Invoke(definitionEntry)
                                                               .OrderBy(d => d.Location.StartLine)
                                                               .ThenBy(d => d.Location.StartCharacter)) {

                    if (context.CancellationToken.IsCancellationRequested) {
                        return;
                    }

                    if (reference.SyntaxTree == null) {
                        continue;
                    }

                    var line = reference.SyntaxTree.SourceText.GetTextLineAtPosition(reference.Location.Start);

                    var lineExtent = line.ExtentWithoutLineEndings;

                    var lineParts = reference.SyntaxTree
                                             .GetClassifiedText(lineExtent)
                                             .ToImmutableArray();

                    var lineHighlightExtent = new TextExtent(start: reference.Start - line.Start,
                                                             length: reference.Location.Length);

                    var previewExtent = GetPreviewExtent(line);

                    var previewParts = reference.SyntaxTree
                                                .GetClassifiedText(previewExtent)
                                                .ToImmutableArray();

                    var previewHighlightExtent = new TextExtent(start: reference.Start - previewExtent.Start,
                                                                length: reference.Location.Length);

                    var item = new ReferenceEntry(definition: definitionEntry,
                                                  location: reference.Location,
                                                  lineParts: lineParts,
                                                  lineHighlightExtent: lineHighlightExtent,
                                                  previewParts: previewParts,
                                                  previewHighlightExtent: previewHighlightExtent);

                    await context.OnReferenceFoundAsync(item);

                }

            });

        }

        const int PreviewLinesOnOneSide = 3;

        static TextExtent GetPreviewExtent(SourceTextLine line) {

            var sourceText = line.SourceText;
            if (sourceText.TextLines.Count == 1) {
                return line.ExtentWithoutLineEndings;
            }

            var lineNumber = line.Line;

            var firstLine = sourceText.TextLines[Math.Max(lineNumber - PreviewLinesOnOneSide, 0)];
            var lastLine  = sourceText.TextLines[Math.Min(lineNumber + PreviewLinesOnOneSide, sourceText.TextLines.Count - 1)];

            return TextExtent.FromBounds(firstLine.Start, lastLine.ExtentWithoutLineEndings.End);

        }

    }

}