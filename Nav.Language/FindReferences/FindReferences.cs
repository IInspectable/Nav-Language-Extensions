#region Using Directives

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

                    // TODO Line Endings..
                    var end = line.End;
                    if (line.End - 2 >= line.Start) {
                        end -= 2;
                    }

                    var classifiedText = reference.SyntaxTree
                                                  .GetClassifiedText(TextExtent.FromBounds(line.Start, end))
                                                  .ToImmutableArray();

                    var highlightExtent = new TextExtent(start: reference.Start - line.Start,
                                                         length: reference.Location.Length);

                    var item = new ReferenceEntry(definitionEntry,
                                                  reference.Location,
                                                  classifiedText,
                                                  highlightExtent);

                    await context.OnReferenceFoundAsync(item);

                }

            });

        }

    }

}