#region Using Directives

using System;
using System.ComponentModel.Composition;

using Microsoft.VisualStudio.Text;
using Pharmatechnik.Nav.Language.CodeFixes.Rename;
using Pharmatechnik.Nav.Language.Extension.CodeFixes;
using Pharmatechnik.Nav.Language.Extension.Common;
using Pharmatechnik.Nav.Language.Extension.Images;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Commands {
    [ExportCommandHandler(CommandHandlerNames.RenameCommandHandler, NavLanguageContentDefinitions.ContentType)]
    class RenameCommandHandler : ICommandHandler<RenameCommandArgs> {

        readonly IDialogService     _dialogService;
        readonly ITextChangeService _textChangeService;

        [ImportingConstructor]
        public RenameCommandHandler(IDialogService dialogService, ITextChangeService textChangeService) {
            _dialogService    = dialogService;
           _textChangeService = textChangeService;
        }

        public CommandState GetCommandState(RenameCommandArgs args, Func<CommandState> nextHandler) {

            var codeGenerationUnitAndSnapshot = TryGetCodeGenerationUnitAndSnapshot(args.SubjectBuffer);
            var symbol = args.TextView.TryFindSymbolUnderCaret(codeGenerationUnitAndSnapshot);

            return symbol != null ? CommandState.Available : nextHandler();
        }

        public void ExecuteCommand(RenameCommandArgs args, Action nextHandler) {

            var codeGenerationUnitAndSnapshot = TryGetCodeGenerationUnitAndSnapshot(args.SubjectBuffer);
            var symbol = args.TextView.TryFindSymbolUnderCaret(codeGenerationUnitAndSnapshot);
            if(symbol == null) {
                nextHandler();
                return;
            }

            var editorSettings = args.TextView.GetEditorSettings();
            var renameCodeFix  = SymbolRenameCodeFix.TryFindCodeFix(symbol, editorSettings, codeGenerationUnitAndSnapshot.CodeGenerationUnit);

            if (renameCodeFix == null || !renameCodeFix.CanApplyFix()) {
                ShellUtil.ShowErrorMessage("You must rename an identifier.");
                return;
            }
            var newSymbolName = _dialogService.ShowInputDialog(
                promptText    : "Name:",
                title         : renameCodeFix.Name,
                defaultResonse: renameCodeFix.Symbol.Name,
                iconMoniker   : ImageMonikers.FromSymbol(symbol),
                validator     : renameCodeFix.ValidateSymbolName
            )?.Trim();

            if (String.IsNullOrEmpty(newSymbolName)) {
                return;
            }

            var textChangesAndSnapshot = new TextChangesAndSnapshot(
                textChanges: renameCodeFix.GetTextChanges(newSymbolName),
                snapshot   : codeGenerationUnitAndSnapshot.Snapshot);

            _textChangeService.ApplyTextChanges(
                textView              : args.TextView, 
                undoDescription       : renameCodeFix.Name,
                textChangesAndSnapshot: textChangesAndSnapshot);

            SemanticModelService.TryGet(args.SubjectBuffer)?.UpdateSynchronously();
        }

        CodeGenerationUnitAndSnapshot TryGetCodeGenerationUnitAndSnapshot(ITextBuffer textBuffer) {
            return SemanticModelService.TryGet(textBuffer)?.CodeGenerationUnitAndSnapshot;
        }
    }
}