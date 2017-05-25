#region Using Directives

using System;
using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen {

    public sealed class TaskExitCodeInfo {

        public TaskExitCodeInfo(IConnectionPointReferenceSymbol connectionPointReferenceSymbol) {

            if (connectionPointReferenceSymbol == null) {
                throw new ArgumentNullException(nameof(connectionPointReferenceSymbol));
            }

            var exitTransition = connectionPointReferenceSymbol.ExitTransition;
            var task = exitTransition.ContainingTask;

            TaskCodeInfo = TaskCodeInfo.FromTaskDefinition(task);
            // TODO PascalCase
            AfterLogicMethodName = $"{CodeGenFacts.ExitMethodPrefix}{exitTransition.Source?.Name}{CodeGenFacts.LogicMethodSuffix}";
        }

        [NotNull]
        public TaskCodeInfo TaskCodeInfo { get; }
        [NotNull]
        public string AfterLogicMethodName { get; }
    }
}