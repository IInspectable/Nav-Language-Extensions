#region Using Directives

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using Pharmatechnik.Nav.Language.Extension.Common;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Commands; 

interface ICommandHandlerServiceProvider {

    ICommandHandlerService GetService(ITextView textView);
    ICommandHandlerService GetService(ITextBuffer textBuffer);
}

[Export(typeof(ICommandHandlerServiceProvider))]
class CommandHandlerServiceProvider : ICommandHandlerServiceProvider {

    readonly IEnumerable<Lazy<INavCommandHandler, CommandHandlerMetadata>> _commandHandlers;

    [ImportingConstructor]
    public CommandHandlerServiceProvider([ImportMany] IEnumerable<Lazy<INavCommandHandler, CommandHandlerMetadata>> commandHandlers) {
        _commandHandlers = commandHandlers;
    }

    public ICommandHandlerService GetService(ITextView textView) {
        var contentTypes    = textView.GetContentTypes();
        var commandHandlers = SelectCommandHandler(contentTypes);
        return new CommandHandlerService(commandHandlers);
    }

    public ICommandHandlerService GetService(ITextBuffer textBuffer) {
        var commandHandlers = SelectCommandHandler(textBuffer.ContentType);
        return new CommandHandlerService(commandHandlers);
    }

    IList<INavCommandHandler> SelectCommandHandler(params IContentType[] contentTypes) {
        return SelectCommandHandler((IEnumerable<IContentType>) contentTypes);
    }

    IList<INavCommandHandler> SelectCommandHandler(IEnumerable<IContentType> contentTypes) {

        var extensions = _commandHandlers.Where(h => contentTypes.Any(d => d.MatchesAny(h.Metadata.ContentTypes)));
                
        var handler = ExtensionOrderer.Order(extensions).Select(ch => ch.Value)
                                      .ToList();

        return handler;
    }
}