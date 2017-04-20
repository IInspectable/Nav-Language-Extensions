#region Using Directives

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Utilities;
using Pharmatechnik.Nav.Language.Extension.Common;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Commands {

    class CommandHandlerMetadata {
       
        public CommandHandlerMetadata(IDictionary<string, object> data) {
            
            ContentTypes = (IReadOnlyList<string>)data.GetValueOrDefault(nameof(ExportCommandHandlerAttribute.ContentTypes)) ?? Array.Empty<string>();
            Before       = (IReadOnlyList<string>)data.GetValueOrDefault(nameof(OrderAttribute.Before)) ?? Array.Empty<string>();
            After        = (IReadOnlyList<string>)data.GetValueOrDefault(nameof(OrderAttribute.After))  ?? Array.Empty<string>();
        }

        public IReadOnlyList<string> ContentTypes { get; }
        public IReadOnlyList<string> Before { get; }
        public IReadOnlyList<string> After { get; }
    }
}