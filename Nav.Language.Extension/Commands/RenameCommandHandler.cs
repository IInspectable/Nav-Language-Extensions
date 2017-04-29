#region Using Directives

using System;
using System.ComponentModel.Composition;

using Microsoft.VisualStudio.Text;

using Pharmatechnik.Nav.Language.Extension.Common;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Commands {
    [ExportCommandHandler(CommandHandlerNames.RenameCommandHandler, NavLanguageContentDefinitions.ContentType)]
    class RenameCommandHandler : ICommandHandler<RenameCommandArgs> {

        [ImportingConstructor]
        public RenameCommandHandler() {
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

            // TODO Renaming
            ShellUtil.ShowInfoMessage("Hi Moritz!");
        }

        SemanticModelResult TryGetSemanticModelResult(ITextBuffer textBuffer) {
            return SemanticModelService.TryGet(textBuffer)?.SemanticModelResult;
        }
    }
}