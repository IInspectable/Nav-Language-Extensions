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
        

        public static ExitTransitionCodeModel FromExitTransition(IExitTransition exitTransition) {

            var taskNode = exitTransition?.Source?.Declaration;
            if(taskNode == null) {
                throw new ArgumentException("Exit transition expected");
            }

            // TODO Result
            var taskResult = new ParameterCodeModel("bool", "todo");
            
            return new ExitTransitionCodeModel(
                calls           : exitTransition.GetReachableCalls().ToImmutableList(), 
                result          : taskResult, 
                taskName        : taskNode.Name);
        }
    }
}