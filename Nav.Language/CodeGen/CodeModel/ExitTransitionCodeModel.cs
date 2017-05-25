#region Using Directives

using System;
using System.Collections.Immutable;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen {

    class ExitTransitionCodeModel : TransitionCodeModel {

        readonly TaskExitCodeInfo _taskExitCodeInfo;

        public ExitTransitionCodeModel(TaskExitCodeInfo taskExitCodeInfo, ImmutableList<Call> calls, 
                                       ParameterCodeModel taskResult)
            :base (calls) {
            _taskExitCodeInfo = taskExitCodeInfo ?? throw new ArgumentNullException(nameof(taskExitCodeInfo));
            TaskResult        = taskResult           ?? throw new ArgumentNullException(nameof(taskResult));
        }

        public ParameterCodeModel TaskResult { get; }

        public string AfterMethodName      => _taskExitCodeInfo.AfterMethodName;
        public string AfterLogicMethodName => _taskExitCodeInfo.AfterLogicMethodName;

        public static ExitTransitionCodeModel FromTaskNode(ITaskNodeSymbol taskNode, TaskCodeInfo taskCodeInfo) {

            if (taskNode == null) {
                throw new ArgumentNullException(nameof(taskNode));
            }

            var taskExitCodeInfo = TaskExitCodeInfo.FromTaskNode(taskNode, taskCodeInfo);
            var calls            = taskNode.GetReachableCalls();
            var taskResult       = ParameterCodeModel.TaskResult(taskNode.Declaration);
            
            return new ExitTransitionCodeModel(
                taskExitCodeInfo: taskExitCodeInfo,
                calls           : calls.ToImmutableList(), 
                taskResult      : taskResult);
        }
    }
}