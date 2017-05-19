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
            var task = exitTransition.ContainingTask;

            TaskCodeModel = TaskCodeModel.FromTaskDefinition(task);
            AfterLogicMethodName = $"{CodeGenFacts.ExitMethodPrefix}{exitTransition.Source?.Name}{CodeGenFacts.LogicMethodSuffix}";
        }

        [NotNull]
        public TaskCodeModel TaskCodeModel { get; }
        [NotNull]
        public string AfterLogicMethodName { get; }
    }
}