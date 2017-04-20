using System;

namespace Pharmatechnik.Nav.Language.Extension.Commands {

    interface ICommandHandler {
    }

    interface ICommandHandler<in T> : ICommandHandler where T : CommandArgs {
        CommandState GetCommandState(T args, Func<CommandState> nextHandler);
        void ExecuteCommand(T args, Action nextHandler);
    }   
}