﻿#region Using Directives

using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition;

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

using Pharmatechnik.Nav.Language.Extension.Common;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Commands.Extensibility {

    interface ICommandHandlerServiceProvider {

        ICommandHandlerService GetService(ITextView textView);
        ICommandHandlerService GetService(ITextBuffer textBuffer);
    }

    [Export(typeof(ICommandHandlerServiceProvider))]
    class CommandHandlerServiceProvider : ICommandHandlerServiceProvider {

        readonly IEnumerable<Lazy<ICommandHandler, CommandHandlerMetadata>> _commandHandlers;

        [ImportingConstructor]
        public CommandHandlerServiceProvider(IEnumerable<Lazy<ICommandHandler, CommandHandlerMetadata>> commandHandlers) {
            _commandHandlers = commandHandlers;
        }

        public ICommandHandlerService GetService(ITextView textView) {
            var contentTypes = textView.GetContentTypes();
            var commandHandlers = SelectCommandHandler(contentTypes);
            return new CommandHandlerService(commandHandlers);
        }

        public ICommandHandlerService GetService(ITextBuffer textBuffer) {
            var commandHandlers = SelectCommandHandler(textBuffer.ContentType);
            return new CommandHandlerService(commandHandlers);
        }

        IList<ICommandHandler> SelectCommandHandler(params IContentType[] contentTypes) {
            return SelectCommandHandler((IEnumerable<IContentType>) contentTypes);
        }

        IList<ICommandHandler> SelectCommandHandler(IEnumerable<IContentType> contentTypes) {
            var handler = _commandHandlers.Where(h => contentTypes.Any(d => d.MatchesAny(h.Metadata.ContentTypes)))
                                          .Select(ch => ch.Value)
                                          .ToList();

            return handler;
        }
    }
}