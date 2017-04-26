#region Using Directives

using System.Threading;
using System.Collections.Generic;

using Microsoft.VisualStudio.Language.Intellisense;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.CodeFixes {

    abstract class CodeFixActionProvider : ICodeFixActionProvider {

        protected CodeFixActionProvider(CodeFixActionContext context) {
            Context = context;
        }

        protected CodeFixActionContext Context { get; }

        public abstract IEnumerable<SuggestedActionSet> GetSuggestedActions(CodeFixActionsParameter parameter, CancellationToken cancellationToken);
    }
}