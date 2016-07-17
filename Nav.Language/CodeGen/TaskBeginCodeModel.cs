#region Using Directives

using System;
using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen {

    public sealed class TaskBeginCodeModel: CodeModel {

        public TaskBeginCodeModel(IInitNodeSymbol initNodeSymbol) {
            if (initNodeSymbol == null) {
                throw new ArgumentNullException(nameof(initNodeSymbol));
            }

            var task  = initNodeSymbol.ContainingTask;
            TaskCodeModel      = new TaskCodeModel(task);
            BeginLogicMethodName = "BeginLogic";
            InitName             = initNodeSymbol.Name??String.Empty;
        }

        [NotNull]
        public TaskCodeModel TaskCodeModel { get; }

        [NotNull]
        public string BeginLogicMethodName { get; }

        [NotNull]
        public string InitName { get; }
    }
}