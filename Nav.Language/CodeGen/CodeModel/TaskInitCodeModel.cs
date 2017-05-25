#region Using Directives

using System;
using System.Collections.Immutable;

using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen {

    public sealed class TaskInitCodeModel {

        TaskInitCodeModel(string initName, TaskCodeInfo taskCodeInfo, ImmutableList<ParameterCodeModel> parameter) {

            // TODO Info aus TaskDeclarationCodeInfo
            TaskCodeInfo        = taskCodeInfo ?? throw new ArgumentNullException(nameof(taskCodeInfo));
            Parameter            = parameter     ?? throw new ArgumentNullException(nameof(parameter));
            BeginMethodName      = $"{CodeGenFacts.BeginMethodPrefix}";
            BeginLogicMethodName = $"{CodeGenFacts.BeginMethodPrefix}{CodeGenFacts.LogicMethodSuffix}";           
            InitName             = initName ?? String.Empty;
        }
        
        public static TaskInitCodeModel FromInitNode(IInitNodeSymbol initNodeSymbol) {

            if (initNodeSymbol == null) {
                throw new ArgumentNullException(nameof(initNodeSymbol));
            }

            var taskCodeModel = TaskCodeInfo.FromTaskDefinition(initNodeSymbol.ContainingTask);

            return FromInitNode(initNodeSymbol, taskCodeModel);
        }

        internal static TaskInitCodeModel FromInitNode(IInitNodeSymbol initNodeSymbol, TaskCodeInfo taskCodeInfo) {

            if (initNodeSymbol == null) {
                throw new ArgumentNullException(nameof(initNodeSymbol));
            }
            if (taskCodeInfo == null) {
                throw new ArgumentNullException(nameof(taskCodeInfo));
            }

            var parameter = ParameterCodeModel.FromParameterSyntaxes(initNodeSymbol.Syntax.CodeParamsDeclaration?.ParameterList);
            
            return new TaskInitCodeModel(initName     : initNodeSymbol.Name ?? String.Empty, 
                                         taskCodeInfo: taskCodeInfo, 
                                         parameter    : parameter.ToImmutableList());
        }

        [NotNull]
        public TaskCodeInfo TaskCodeInfo { get; }
        [NotNull]
        public string BeginLogicMethodName { get; }
        [NotNull]
        public string BeginMethodName { get; }
        [NotNull]
        public string InitName { get; }
        [NotNull]
        ImmutableList<ParameterCodeModel> Parameter { get; }
    }
}