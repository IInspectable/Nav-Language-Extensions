#region Using Directives

using System.ComponentModel.Composition;

using Microsoft.VisualStudio.Commanding;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Editor.Commanding.Commands;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Utilities;

using Pharmatechnik.Nav.Language.Extension.Common;
using Pharmatechnik.Nav.Language.Extension.FindReferences;
using Pharmatechnik.Nav.Language.FindReferences;

using Task = System.Threading.Tasks.Task;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Commands {

    [Export(typeof(ICommandHandler))]
    [ContentType(NavLanguageContentDefinitions.ContentType)]
    [Name(CommandHandlerNames.FindReferencesCommandHandler)]
    class FindReferencesCommandHandler: ICommandHandler<FindReferencesCommandArgs> {

        readonly FindReferencesPresenter _referencesPresenter;

        [ImportingConstructor]
        public FindReferencesCommandHandler(FindReferencesPresenter referencesPresenter) {
            _referencesPresenter = referencesPresenter;

        }

        public string DisplayName => "Find All References";

        public CommandState GetCommandState(FindReferencesCommandArgs args) {
            return args.TextView is IWpfTextView ? CommandState.Available : CommandState.Unavailable;
        }

        public bool ExecuteCommand(FindReferencesCommandArgs args, CommandExecutionContext executionContext) {

            FindAllReferencesAsync(args).FileAndForget("nav/extension/findreferences");

            return false;

        }

        async Task FindAllReferencesAsync(FindReferencesCommandArgs args) {

            var codeGenerationUnitAndSnapshot = GetCodeGenerationUnit(args.SubjectBuffer);
            var originatingSymbol             = args.TextView.TryFindSymbolUnderCaret(codeGenerationUnitAndSnapshot);

            var context = _referencesPresenter.StartSearch();
            try {

                if (originatingSymbol == null) {
                    // Search found no results.
                    return;
                }

                var solution = await NavLanguagePackage.GetSolutionAsync(context.CancellationToken);
                var fra      = new FindReferencesArgs(originatingSymbol, codeGenerationUnitAndSnapshot.CodeGenerationUnit, solution, context);

                await ReferenceFinder.FindReferencesAsync(fra).ConfigureAwait(false);

            } finally {
                await context.OnCompletedAsync();
            }

        }

        static CodeGenerationUnitAndSnapshot GetCodeGenerationUnit(ITextBuffer textBuffer) {

            var semanticModelService      = SemanticModelService.GetOrCreateSingelton(textBuffer);
            var generationUnitAndSnapshot = semanticModelService.UpdateSynchronously();

            return generationUnitAndSnapshot;
        }

    }

}