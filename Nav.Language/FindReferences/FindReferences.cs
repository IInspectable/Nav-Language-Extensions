#region Using Directives

using System.Threading.Tasks;

#endregion

namespace Pharmatechnik.Nav.Language.FindReferences {

    public class ReferenceFinder {

        public static Task FindReferences(ISymbol symbol, IFindReferencesContext context) {

            if (symbol == null) {
                return Task.CompletedTask;
            }

            return Task.Run(async () => {

                var definition = FindRootDefinitionVisitor.Invoke(symbol);
                if (definition == null) {
                    return;
                }

                foreach (var reference in FindReferencesVisitor.Invoke(definition)) {
                    if (context.CancellationToken.IsCancellationRequested) {
                        return;
                    }

                    var item = new ReferenceEntry(definition,
                                                  reference.Location,
                                                  reference.Name);

                    await context.OnReferenceFoundAsync(item);

                }

            });

        }

    }

}