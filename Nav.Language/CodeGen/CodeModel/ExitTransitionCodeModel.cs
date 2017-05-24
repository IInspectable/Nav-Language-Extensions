#region Using Directives

using System;
using System.Collections.Immutable;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen {

    class ExitTransitionCodeModel : TransitionCodeModel {

        public ExitTransitionCodeModel(ImmutableList<Call> calls, 
                                       ParameterCodeModel result, string taskName)
            :base (calls) {
         
            TaskName   = taskName ?? String.Empty;
            TaskResult = result   ?? throw new ArgumentNullException(nameof(result));
        }

        public ParameterCodeModel TaskResult { get; }
        public string TaskName { get; }
        public string TaskNamePascalcase => TaskName.ToPascalcase();
        

        public static ExitTransitionCodeModel FromTaskNode(ITaskNodeSymbol taskNode) {
            if(taskNode == null) {
                throw new ArgumentNullException(nameof(taskNode));
            }

            
            var taskResult = ParameterCodeModel.TaskResult(taskNode.Declaration);
            
            return new ExitTransitionCodeModel(
                calls           : taskNode.GetReachableCalls().ToImmutableList(), 
                result          : taskResult, 
                taskName        : taskNode.Name);
        }
    }
}