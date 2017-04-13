#region Using Directives

using System;
using System.Linq;
using System.Collections.Generic;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Commands.Extensibility {

    interface ICommandHandlerService {
        CommandState GetCommandState<T>(T args, Func<CommandState> lastHandler) where T : CommandArgs;
        void Execute<T>(T args, Action lastHandler) where T : CommandArgs;
    }

    class CommandHandlerService : ICommandHandlerService {

        readonly IList<ICommandHandler> _commandHandlers;
        readonly Dictionary<Type, object> _commandHandlersByType;

        public CommandHandlerService(IList<ICommandHandler> commandHandlers) {
            _commandHandlers       = commandHandlers;
            _commandHandlersByType = new Dictionary<Type, object>();
        }

        public CommandState GetCommandState<T>(T args, Func<CommandState> lastHandler) where T : CommandArgs {

            var handlers = GetCommandHandlers<T>();
            return GetCommandState(handlers, args, lastHandler);
        }

        public void Execute<T>(T args, Action lastHandler) where T : CommandArgs {
            var handlers = GetCommandHandlers<T>();
            ExecuteHandlers(handlers, args, lastHandler);
        }

        IList<ICommandHandler<T>> GetCommandHandlers<T>() where T : CommandArgs {

            var key = typeof(T);
            if(!_commandHandlersByType.TryGetValue(key, out var commandHandlerList)) {
                var stronglyTypedHandlers = _commandHandlers.OfType<ICommandHandler<T>>().ToList();
                _commandHandlersByType.Add(key, stronglyTypedHandlers);
            }

            return (IList<ICommandHandler<T>>) commandHandlerList;
        }

        static CommandState GetCommandState<TArgs>(
            IList<ICommandHandler<TArgs>> commandHandlers,
            TArgs args,
            Func<CommandState> lastHandler) where TArgs : CommandArgs {

            if(commandHandlers.Count > 0) {
                // Build up chain of handlers.
                var handlerChain = lastHandler ?? (() => default(CommandState));
                for(int i = commandHandlers.Count - 1; i >= 1; i--) {
                    // Declare locals to ensure that we don't end up capturing the wrong thing
                    var nextHandler = handlerChain;
                    int j = i;
                    handlerChain = () => commandHandlers[j].GetCommandState(args, nextHandler);
                }

                // Kick off the first command handler.
                return commandHandlers[0].GetCommandState(args, handlerChain);
            }

            if (lastHandler != null) {
                // If there aren't any command handlers, just invoke the last handler (if there is one).
                return lastHandler();
            }

            return CommandState.Unavailable;
        }

        static void ExecuteHandlers<T>(IList<ICommandHandler<T>> commandHandlers, T args, Action lastHandler) where T : CommandArgs {
            if(commandHandlers?.Count > 0) {
                // Build up chain of handlers.
                var handlerChain = lastHandler ?? delegate { };
                for(int i = commandHandlers.Count - 1; i >= 1; i--) {
                    // Declare locals to ensure that we don't end up capturing the wrong thing
                    var nextHandler = handlerChain;
                    int j = i;
                    handlerChain = () => commandHandlers[j].ExecuteCommand(args, nextHandler);
                }

                // Kick off the first command handler.
                commandHandlers[0].ExecuteCommand(args, handlerChain);
            } else {
                // If there aren't any command handlers, just invoke the last handler (if there is one).
                lastHandler?.Invoke();
            }
        }
    }
}