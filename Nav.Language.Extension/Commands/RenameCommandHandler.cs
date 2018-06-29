﻿#region Using Directives

using System;
using System.Linq;
using System.ComponentModel.Composition;

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.VisualStudio.Shell;

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
            // Das Ändern der Caret-Position veranlasst kein erneutes Abfragen des Commandstates.
            // Das hat zur Folge, dass z.B. beim Drücken der Taste F2 auf einem Keyword der Rename
            // Befehl so lange deaktiviert wird, bis auf einem Symbol das Kontextmenü aufgerufen, und 
            // dadurch der Commandstate erneut (positiv) abgefragt wird.
            // Deswegen retunieren wir hier grundsätzlich "Available".
            // var codeGenerationUnitAndSnapshot = TryGetCodeGenerationUnitAndSnapshot(args.SubjectBuffer);
            // var symbol = args.TextView.TryFindSymbolUnderCaret(codeGenerationUnitAndSnapshot);
            return CommandState.Available;
        }

        public void ExecuteCommand(RenameCommandArgs args, Action nextHandler) {

            ThreadHelper.ThrowIfNotOnUIThread();

            var codeGenerationUnitAndSnapshot = TryGetCodeGenerationUnitAndSnapshot(args.SubjectBuffer);
            if (!codeGenerationUnitAndSnapshot.IsCurrent(args.SubjectBuffer.CurrentSnapshot)) {
                // TODO Messagebox mit Grund anzeigen?
                nextHandler();
                return;
            }

            var symbol = args.TextView.TryFindSymbolUnderCaret(codeGenerationUnitAndSnapshot);
            if (symbol == null) {
                // TODO Messagebox mit Grund anzeigen?
                nextHandler();
                return;
            }

            //if (symbol is ISignalTriggerSymbol signaltrigger) {

            //    var project = args.TextView.TextBuffer.GetContainingProject();
            //    if (project == null) {
            //        return;
            //    }

            //    var codeGenInfo   = SignalTriggerCodeInfo.FromSignalTrigger(signaltrigger);
            //    var triggerMethod = await LocationFinder.FindTriggerMethodSymbol(project, codeGenInfo, CancellationToken.None);

            //    var originalSolution = project.Solution;
            //    var workspace        = originalSolution.Workspace;
            //    var optionSet        = workspace.Options;

            //    var solution = await Renamer.RenameSymbolAsync(originalSolution, triggerMethod, "OnFoo", optionSet, CancellationToken.None);

            //    workspace.TryApplyChanges(solution);

            //    return;
            //}

            var codeFixContext = new CodeFixContext(
                    symbol.Location.Extent, 
                    codeGenerationUnitAndSnapshot.CodeGenerationUnit, 
                    args.TextView.GetEditorSettings());
            
            var renameCodeFix  = RenameCodeFixProvider.SuggestCodeFixes(codeFixContext).FirstOrDefault();

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