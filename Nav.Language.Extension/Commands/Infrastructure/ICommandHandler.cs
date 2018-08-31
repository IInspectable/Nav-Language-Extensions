using System;

namespace Pharmatechnik.Nav.Language.Extension.Commands {

    interface INavCommandHandler {
    }

    interface INavCommandHandler<in T> : INavCommandHandler where T : CommandArgs {
        NavCommandState GetCommandState(T args, Func<NavCommandState> nextHandler);
        void ExecuteCommand(T args, Action nextHandler);
    }   
}