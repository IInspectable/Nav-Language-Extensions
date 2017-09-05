#region Using Directives

using System;

using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen {

    public sealed class TaskInitCodeInfo {

        TaskInitCodeInfo(string initName, TaskCodeInfo taskCodeInfo) {

            // TODO Info aus TaskDeclarationCodeInfo
            TaskCodeInfo         = taskCodeInfo ?? throw new ArgumentNullException(nameof(taskCodeInfo));
            BeginMethodName      = $"{CodeGenFacts.BeginMethodPrefix}";
            BeginLogicMethodName = $"{CodeGenFacts.BeginMethodPrefix}{CodeGenFacts.LogicMethodSuffix}";           
            InitName             = initName ?? String.Empty;
        }

        [NotNull]
        public TaskCodeInfo TaskCodeInfo { get; }
        [NotNull]
        public string BeginLogicMethodName { get; }
        [NotNull]
        public string BeginMethodName { get; }
        [NotNull]
        public string InitName { get; }

        public static TaskInitCodeInfo FromInitNode(IInitNodeSymbol initNodeSymbol) {

            if (initNodeSymbol == null) {
                throw new ArgumentNullException(nameof(initNodeSymbol));
            }

            var taskCodeModel = TaskCodeInfo.FromTaskDefinition(initNodeSymbol.ContainingTask);

            return FromInitNode(initNodeSymbol, taskCodeModel);
        }

        internal static TaskInitCodeInfo FromInitNode(IInitNodeSymbol initNodeSymbol, TaskCodeInfo taskCodeInfo) {

            if (initNodeSymbol == null) {
                throw new ArgumentNullException(nameof(initNodeSymbol));
            }
            if (taskCodeInfo == null) {
                throw new ArgumentNullException(nameof(taskCodeInfo));
            }
            
            return new TaskInitCodeInfo(initName    : initNodeSymbol.Name ?? String.Empty, 
                                        taskCodeInfo: taskCodeInfo);
        }       
    }
}