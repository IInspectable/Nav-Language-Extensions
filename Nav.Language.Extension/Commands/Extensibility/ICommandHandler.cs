using System;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace Pharmatechnik.Nav.Language.Extension.Commands.Extensibility {

    interface ICommandHandler {
    }

    interface ICommandHandler<in T> : ICommandHandler where T : CommandArgs {
        CommandState GetCommandState(T args, Func<CommandState> nextHandler);
        void ExecuteCommand(T args, Action nextHandler);
    }

    internal interface ICommandHandlerService {
        CommandState GetCommandState<T>(IContentType contentType, T args, Func<CommandState> lastHandler = null) where T : CommandArgs;
        void Execute<T>(IContentType contentType, T args, Action lastHandler = null) where T : CommandArgs;
    }
    interface ICommandHandlerServiceFactory {
        ICommandHandlerService GetService(ITextView textView);
        ICommandHandlerService GetService(ITextBuffer textBuffer);
        void Initialize(string contentTypeName);
    }
}