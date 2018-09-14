#region Using Directives

using System.Collections.Immutable;
using System.Threading.Tasks;

using Pharmatechnik.Nav.Language.Text;

#endregion

namespace Pharmatechnik.Nav.Language.FindReferences {

    public class ReferenceFinder {

        public static Task FindReferences(ISymbol symbol, IFindReferencesContext context) {

            if (symbol == null) {
                return Task.CompletedTask;
            }

            return Task.Run(async () => {

                var definitionRoot = FindRootDefinitionVisitor.Invoke(symbol);
                if (definitionRoot == null) {
                    return;
                }

                var parts = new[] {
                    ClassifiedText.Keyword(SyntaxFacts.TaskKeyword),
                    ClassifiedText.Space,
                    ClassifiedText.TaskName(definitionRoot.Name)
                };
                
                var definition = new DefinitionEntry(parts.ToImmutableArray());

                foreach (var reference in FindReferencesVisitor.Invoke(definitionRoot)) {
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