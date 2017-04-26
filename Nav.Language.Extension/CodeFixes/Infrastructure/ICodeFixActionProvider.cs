#region Using Directives

using System.Threading;
using System.Collections.Generic;
using Microsoft.VisualStudio.Language.Intellisense;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.CodeFixes {
    
    interface ICodeFixActionProvider {
        IEnumerable<SuggestedActionSet> GetSuggestedActions(CodeFixActionsParameter parameter, CancellationToken cancellationToken);
    }
}
