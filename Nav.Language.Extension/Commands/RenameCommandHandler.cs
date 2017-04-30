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

            var semanticModelResult = TryGetSemanticModelResult(args.SubjectBuffer);
            var symbol = args.TextView.TryFindSymbolUnderCaret(semanticModelResult);

            return symbol != null ? CommandState.Available : nextHandler();
        }

        public void ExecuteCommand(RenameCommandArgs args, Action nextHandler) {

            var semanticModelResult = TryGetSemanticModelResult(args.SubjectBuffer);
            var symbol = args.TextView.TryFindSymbolUnderCaret(semanticModelResult);
            if(symbol == null) {
                nextHandler();
                return;
            }

            var editorSettings = args.TextView.GetEditorSettings();
            var renameCodeFix  = Renamer.TryFindRenameCodeFix(symbol, editorSettings, semanticModelResult.CodeGenerationUnit);

            if (renameCodeFix == null || !renameCodeFix.CanApplyFix()) {
                ShellUtil.ShowErrorMessage("You must rename an identifier.");
                return;
            }
            var newSymbolName = _dialogService.ShowInputDialog(
                promptText    : "Name:",
                title         : renameCodeFix.DisplayText,
                defaultResonse: renameCodeFix.Symbol.Name,
                iconMoniker   : ImageMonikers.FromSymbol(symbol),
                validator     : renameCodeFix.ValidateSymbolName
            )?.Trim();

            if (String.IsNullOrEmpty(newSymbolName)) {
                return;
            }

            var textChanges = renameCodeFix.GetTextChanges(newSymbolName);

            _textChangeService.ApplyTextChanges(
                textView          : args.TextView, 
                undoDescription   : renameCodeFix.DisplayText, 
                textChanges       : textChanges, 
                textChangeSnapshot: semanticModelResult.Snapshot);            
        }

        SemanticModelResult TryGetSemanticModelResult(ITextBuffer textBuffer) {
            return SemanticModelService.TryGet(textBuffer)?.SemanticModelResult;
        }
    }
}