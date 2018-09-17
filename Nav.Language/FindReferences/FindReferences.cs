#region Using Directives

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

                    var item = new ReferenceEntry(definitionEntry,
                                                  reference.Location,
                                                  reference.Name);

                    await context.OnReferenceFoundAsync(item);

                }

            });

        }

    }

}