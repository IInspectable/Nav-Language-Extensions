#region Using Directives

using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

using Microsoft.VisualStudio.Commanding;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Editor.Commanding.Commands;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Threading;
using Microsoft.VisualStudio.Utilities;

using Pharmatechnik.Nav.Language.CodeAnalysis.FindReferences;
using Pharmatechnik.Nav.Language.Extension.Common;
using Pharmatechnik.Nav.Language.Extension.FindReferences;
using Pharmatechnik.Nav.Language.FindReferences;

using Task = System.Threading.Tasks.Task;
using ThreadHelper = Microsoft.VisualStudio.Shell.ThreadHelper;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Commands; 

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

        ThreadHelper.ThrowIfNotOnUIThread();

        var codeGenerationUnitAndSnapshot = GetCodeGenerationUnit(args.SubjectBuffer);
        var context                       = _referencesPresenter.StartSearch();

        FindAllReferencesAsync(args, codeGenerationUnitAndSnapshot, context).FileAndForget("nav/extension/findreferences");

        return false;

    }

    async Task FindAllReferencesAsync(FindReferencesCommandArgs args, CodeGenerationUnitAndSnapshot codeGenerationUnitAndSnapshot, FindReferencesContext context) {

        var originatingSymbol = args.TextView.TryFindSymbolUnderCaret(codeGenerationUnitAndSnapshot);

        try {

            if (originatingSymbol == null) {
                // Search found no results.
                return;
            }

            // switch to a background thread
            await TaskScheduler.Default;

            var solution = await NavLanguagePackage.GetSolutionAsync(context.CancellationToken).ConfigureAwait(false);
            var fra      = new FindReferencesArgs(originatingSymbol, codeGenerationUnitAndSnapshot.CodeGenerationUnit, solution, context);

            await ReferenceFinder.FindReferencesAsync(fra).ConfigureAwait(false);
            await WfsReferenceFinder.FindReferencesAsync(NavLanguagePackage.Workspace.CurrentSolution, fra).ConfigureAwait(false);

        } catch (OperationCanceledException) {
        } finally {
            await context.OnCompletedAsync();
        }

    }

    static CodeGenerationUnitAndSnapshot GetCodeGenerationUnit(ITextBuffer textBuffer) {

        ThreadHelper.ThrowIfNotOnUIThread();

        var semanticModelService      = SemanticModelService.GetOrCreateSingelton(textBuffer);
        var generationUnitAndSnapshot = semanticModelService.UpdateSynchronously();

        return generationUnitAndSnapshot;
    }

}