#region Using Directives

using System;
using System.Threading.Tasks;

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

                var solutionRoot   = args.SolutionRoot?.FullName ?? definition.SyntaxTree?.SourceText.FileInfo?.DirectoryName ?? "";
                var definitionItem = new DefinitionItem(solutionRoot, definition, definition.ToDisplayParts());

                await args.Context.OnDefinitionFoundAsync(definitionItem);

                foreach (var reference in FindReferencesVisitor.Invoke(args, definitionItem)) {

                    if (args.Context.CancellationToken.IsCancellationRequested) {
                        return;
                    }

                    var referenceItem = ReferenceItemBuilder.Invoke(definitionItem, reference);

                    if (referenceItem == null) {
                        continue;
                    }

                    await args.Context.OnReferenceFoundAsync(referenceItem);

                }

            });

        }

    }

}