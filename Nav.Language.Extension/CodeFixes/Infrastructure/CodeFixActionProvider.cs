#region Using Directives

using System.Threading;
using System.Collections.Generic;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.CodeFixes {

    interface ICodeFixActionProvider {
        IEnumerable<CodeFixSuggestedAction> GetSuggestedActions(CodeFixActionsParameter parameter, CancellationToken cancellationToken);
    }

    abstract class CodeFixActionProvider : ICodeFixActionProvider {

        protected CodeFixActionProvider(CodeFixActionContext context) {
            Context = context;
        }

        protected CodeFixActionContext Context { get; }

        public abstract IEnumerable<CodeFixSuggestedAction> GetSuggestedActions(CodeFixActionsParameter parameter, CancellationToken cancellationToken);
    }
}