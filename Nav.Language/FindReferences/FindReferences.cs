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

                var definitionEntry= new DefinitionEntry(definition, definition.ToDisplayParts());
               

                foreach (var reference in FindReferencesVisitor.Invoke(definitionEntry)
                                                               .OrderBy(d => d.Location.StartLine)
                                                               .ThenBy(d => d.Location.StartCharacter)) {

                    if (context.CancellationToken.IsCancellationRequested) {
                        return;
                    }


                    var displayParts = ImmutableArray<ClassifiedText>.Empty;
                    
                    // TODO Klassifizieren
                    if (reference.SyntaxTree != null) {
                        var line = reference.SyntaxTree.SourceText.GetTextLineAtPosition(reference.Location.Start);
                        // TODO Line Endings..
                        displayParts = displayParts.Add(ClassifiedTexts.Keyword("Max "));
                        displayParts = displayParts.Add(ClassifiedTexts.Identifier(line.ToString().TrimEnd('\r', '\n')));
                    } else {
                        displayParts = displayParts.Add(ClassifiedTexts.Identifier(reference.Name));
                    }

                    var item = new ReferenceEntry(definitionEntry,
                                                  reference.Location,
                                                  displayParts);

                    await context.OnReferenceFoundAsync(item);

                }

            });

        }

    }

}