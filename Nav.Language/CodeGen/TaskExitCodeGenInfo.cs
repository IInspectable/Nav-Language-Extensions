#region Using Directives

using System;
using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen {

    public sealed class TaskExitCodeGenInfo {

        public TaskExitCodeGenInfo(IConnectionPointReferenceSymbol connectionPointReferenceSymbol) {

            if (connectionPointReferenceSymbol == null) {
                throw new ArgumentNullException(nameof(connectionPointReferenceSymbol));
            }

            var exitTransition = connectionPointReferenceSymbol.ExitTransition;
            var task = exitTransition.TaskDefinition;

            TaskCodeGenInfo = new TaskCodeGenInfo(task);
            AfterLogicMethodName = $"After{exitTransition.Source?.Name}Logic";
        }

        [NotNull]
        public TaskCodeGenInfo TaskCodeGenInfo { get; }

        [NotNull]
        public string AfterLogicMethodName { get; }
    }
}