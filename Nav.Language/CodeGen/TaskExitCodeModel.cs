#region Using Directives

using System;
using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen {

    public sealed class TaskExitCodeModel: CodeModel {

        public TaskExitCodeModel(IConnectionPointReferenceSymbol connectionPointReferenceSymbol) {

            if (connectionPointReferenceSymbol == null) {
                throw new ArgumentNullException(nameof(connectionPointReferenceSymbol));
            }

            var exitTransition = connectionPointReferenceSymbol.ExitTransition;
            var task = exitTransition.TaskDefinition;

            TaskCodeModel = new TaskCodeModel(task);
            AfterLogicMethodName = $"After{exitTransition.Source?.Name}Logic";
        }

        [NotNull]
        public TaskCodeModel TaskCodeModel { get; }

        [NotNull]
        public string AfterLogicMethodName { get; }
    }
}