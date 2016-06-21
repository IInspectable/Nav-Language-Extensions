#region Using Directives

using System;
using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen {
    public sealed class TaskBeginCodeGenInfo {

        public TaskBeginCodeGenInfo(IInitNodeSymbol initNodeSymbol) {
            if (initNodeSymbol == null) {
                throw new ArgumentNullException(nameof(initNodeSymbol));
            }

            var task  = initNodeSymbol.ContainingTask;
            TaskCodeGenInfo      = new TaskCodeGenInfo(task);
            BeginLogicMethodName = "BeginLogic";
            InitName             = initNodeSymbol.Name??String.Empty;
        }

        [NotNull]
        public TaskCodeGenInfo TaskCodeGenInfo { get; }

        [NotNull]
        public string BeginLogicMethodName { get; }

        [NotNull]
        public string InitName { get; }
    }
}