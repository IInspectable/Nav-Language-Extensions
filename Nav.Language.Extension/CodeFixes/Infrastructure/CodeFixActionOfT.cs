#region Using Directives

using System;
using Pharmatechnik.Nav.Language.CodeFixes;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.CodeFixes {

    abstract class CodeFixSuggestedAction<T> : CodeFixSuggestedAction where T : CodeFix {

        protected CodeFixSuggestedAction(CodeFixActionContext context, CodeFixActionsParameter parameter, T codeFix) : base(context, parameter) {
            CodeFix = codeFix ?? throw new ArgumentNullException(nameof(codeFix));
        }

        public T CodeFix { get; }
        public override string UndoDescription => CodeFix.Name;
    }
}