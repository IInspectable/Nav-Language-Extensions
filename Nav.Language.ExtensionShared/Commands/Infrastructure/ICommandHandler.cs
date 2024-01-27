using System;

using Microsoft.VisualStudio.Commanding;

namespace Pharmatechnik.Nav.Language.Extension.Commands; 

interface INavCommandHandler;

interface INavCommandHandler<in T> : INavCommandHandler where T : CommandArgs {
    CommandState GetCommandState(T args, Func<CommandState> nextHandler);
    void ExecuteCommand(T args, Action nextHandler);
}