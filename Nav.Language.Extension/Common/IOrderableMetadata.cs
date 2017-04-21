#region Using Directives

using System.Collections.Generic;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Common {
    interface IOrderableMetadata {
        string Name { get; }
        IReadOnlyList<string> Before { get; }
        IReadOnlyList<string> After { get; }
    }
}