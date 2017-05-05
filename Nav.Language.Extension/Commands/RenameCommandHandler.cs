#region Using Directives

using System;
using System.Linq;
using System.ComponentModel.Composition;

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Imaging.Interop;

using Pharmatechnik.Nav.Language.CodeFixes;
using Pharmatechnik.Nav.Language.CodeFixes.Rename;

using Pharmatechnik.Nav.Language.Extension.Common;
using Pharmatechnik.Nav.Language.Extension.Images;
using Pharmatechnik.Nav.Language.Extension.CodeFixes;

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
            if (!codeGenerationUnitAndSnapshot.IsCurrent(args.SubjectBuffer.CurrentSnapshot)) {
                nextHandler();
                return;
            }

            var symbol = args.TextView.TryFindSymbolUnderCaret(codeGenerationUnitAndSnapshot);
            if (symbol == null) {
                nextHandler();
                return;
            }
          
            var codeFixContext = new CodeFixContext(
                    symbol.Location.Extent, 
                    codeGenerationUnitAndSnapshot.CodeGenerationUnit, 
                    args.TextView.GetEditorSettings());
            
            var renameCodeFix  = RenameCodeFixProvider.TryGetCodeFix(codeFixContext).FirstOrDefault();

            if (renameCodeFix == null) {
                // TODO In IDialogService?
                ShellUtil.ShowErrorMessage("You must rename an identifier.");
                return;
            }

            string note         = null;
            var noteIconMoniker = default(ImageMoniker);
            if (renameCodeFix.Impact != CodeFixImpact.None) {
                // TODO Text
                note            = "Renaming this symbol might break existing code!";
                noteIconMoniker = ImageMonikers.FromCodeFixImpact(renameCodeFix.Impact);
            }

            var newSymbolName = _dialogService.ShowInputDialog(
                promptText     : "Name:",
                title          : renameCodeFix.Name,
                defaultResonse : renameCodeFix.ProvideDefaultName(),
                iconMoniker    : ImageMonikers.FromSymbol(symbol),
                validator      : renameCodeFix.ValidateSymbolName,
                noteIconMoniker: noteIconMoniker,
                note           : note
                
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

            // TODO Selection Logik?
        }

        CodeGenerationUnitAndSnapshot TryGetCodeGenerationUnitAndSnapshot(ITextBuffer textBuffer) {
            return SemanticModelService.TryGet(textBuffer)?.CodeGenerationUnitAndSnapshot;
        }
    }
}