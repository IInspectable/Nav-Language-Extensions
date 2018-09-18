#region Using Directives

using System.ComponentModel.Composition;
using System.Threading.Tasks;

using Microsoft.VisualStudio.Commanding;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Editor.Commanding.Commands;
using Microsoft.VisualStudio.Utilities;

using Pharmatechnik.Nav.Language.Extension.Common;
using Pharmatechnik.Nav.Language.Extension.FindReferences;
using Pharmatechnik.Nav.Language.FindReferences;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Commands {

    [Export(typeof(ICommandHandler))]
    [ContentType(NavLanguageContentDefinitions.ContentType)]
    [Name(CommandHandlerNames.FindReferencesCommandHandler)]
    class FindReferencesCommandHandler: ICommandHandler<FindReferencesCommandArgs> {

        private readonly FindReferencesPresenter _referencesPresenter;

        [ImportingConstructor]
        public FindReferencesCommandHandler(FindReferencesPresenter referencesPresenter) {
            _referencesPresenter = referencesPresenter;

        }

        public string DisplayName => "Find All References";

        public CommandState GetCommandState(FindReferencesCommandArgs args) {
            return args.TextView is IWpfTextView ? CommandState.Available : CommandState.Unavailable;
        }

        public bool ExecuteCommand(FindReferencesCommandArgs args, CommandExecutionContext executionContext) {

            var codeGenerationUnitAndSnapshot = GetCodeGenerationUnit(args.SubjectBuffer);
            var symbol                        = args.TextView.TryFindSymbolUnderCaret(codeGenerationUnitAndSnapshot);

            var _ = FindAllReferencesAsync(symbol);

            return false;

        }

        async Task FindAllReferencesAsync(ISymbol symbol) {

            if (symbol == null) {
                return;
            }

            var args    = new FindReferencesArgs(symbol, NavLanguagePackage.SearchDirectory);
            var context = _referencesPresenter.StartSearch();

            await ReferenceFinder.FindReferences(args, context);
            await context.OnCompletedAsync();

        }

        static CodeGenerationUnitAndSnapshot GetCodeGenerationUnit(ITextBuffer textBuffer) {

            var semanticModelService      = SemanticModelService.GetOrCreateSingelton(textBuffer);
            var generationUnitAndSnapshot = semanticModelService.UpdateSynchronously();

            return generationUnitAndSnapshot;
        }

    }

}