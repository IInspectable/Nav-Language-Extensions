#region Using Directives

using System;
using Pharmatechnik.Nav.Language.CodeFixes;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.CodeFixes {

    abstract class CodeFixAction<T> : CodeFixAction where T : CodeFix {

        protected CodeFixAction(CodeFixActionContext context, CodeFixActionsParameter parameter, T codeFix) : base(context, parameter) {
            CodeFix = codeFix ?? throw new ArgumentNullException(nameof(codeFix));
        }

        public T CodeFix { get; }
        public override string UndoDescription => CodeFix.Name;
    }
}