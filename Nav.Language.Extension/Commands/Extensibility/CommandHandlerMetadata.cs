#region Using Directives

using System.Collections.Generic;
using Pharmatechnik.Nav.Language.Extension.Common;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Commands.Extensibility {

    class CommandHandlerMetadata {

        public IEnumerable<string> ContentTypes { get; }

        public CommandHandlerMetadata(IDictionary<string, object> data) {
            ContentTypes = (IEnumerable<string>) data.GetValueOrDefault("ContentTypes");
        }
    }
}