using System;

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

        public TaskCodeGenInfo TaskCodeGenInfo { get; }
        public string AfterLogicMethodName { get; }

    }
}