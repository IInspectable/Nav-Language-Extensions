#region Using Directives

using System;
using Pharmatechnik.Nav.Language.Extension.Commands.Extensibility;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Commands {

    [ExportCommandHandler("Comment Selection Command Handler", NavLanguageContentDefinitions.ContentType)]
    internal class CommentUncommentSelectionCommandHandler :
        ICommandHandler<CommentSelectionCommandArgs>,
        ICommandHandler<UncommentSelectionCommandArgs> {

        public CommandState GetCommandState(CommentSelectionCommandArgs args, Func<CommandState> nextHandler) {
            // TODO Implement GetCommandState
            return nextHandler();
        }

        public void ExecuteCommand(CommentSelectionCommandArgs args, Action nextHandler) {
            // TODO Implement ExecuteCommand
            nextHandler();
        }

        public CommandState GetCommandState(UncommentSelectionCommandArgs args, Func<CommandState> nextHandler) {
            // TODO Implement GetCommandState
            return nextHandler();
        }

        public void ExecuteCommand(UncommentSelectionCommandArgs args, Action nextHandler) {
            // TODO Implement ExecuteCommand
            nextHandler();
        }

        // TODO AutoClosingViewProperty
    }
}
