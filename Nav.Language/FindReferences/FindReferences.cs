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

                var searchRoot     = args.SearchDirectory?.FullName ?? definition.SyntaxTree?.SourceText.FileInfo?.FullName ?? "";
                var definitionItem = new DefinitionItem(searchRoot, definition, definition.ToDisplayParts());

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