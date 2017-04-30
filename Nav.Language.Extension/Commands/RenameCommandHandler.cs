#region Using Directives

using System;
using System.ComponentModel.Composition;

using Microsoft.VisualStudio.Text;
using Pharmatechnik.Nav.Language.CodeAnalysis.CodeFixes.Rename;
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
            var codeFix=Renamer.TryFindRenameCodeFix(symbol, editorSettings, semanticModelResult.CodeGenerationUnit);
            if(codeFix == null || !codeFix.CanApplyFix()) {
                ShellUtil.ShowErrorMessage("You must rename an identifier.");
                return;
            }
            // Generischer Text
            var newSymbolName = _dialogService.ShowInputDialog(
                promptText    : "Name:",
                title         : "Rename choice",
                defaultResonse: codeFix.Symbol.Name,
                iconMoniker   : ImageMonikers.ChoiceNode,
                validator     : codeFix.ValidateSymbolName
            )?.Trim();

            if (String.IsNullOrEmpty(newSymbolName)) {
                return;
            }

            var textChanges = codeFix.GetTextChanges(newSymbolName);

            _textChangeService.ApplyTextChanges(args.TextView, "", "", textChanges, semanticModelResult.Snapshot);            
        }

        SemanticModelResult TryGetSemanticModelResult(ITextBuffer textBuffer) {
            return SemanticModelService.TryGet(textBuffer)?.SemanticModelResult;
        }
    }
}