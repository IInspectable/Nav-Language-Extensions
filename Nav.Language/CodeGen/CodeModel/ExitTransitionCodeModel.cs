#region Using Directives

using System;
using System.Collections.Immutable;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen {
    class ExitTransitionCodeModel : TransitionCodeModel {
        
        public ExitTransitionCodeModel(ImmutableList<CallCodeModel> targetNodes, ParameterCodeModel result, string taskName)
            :base (targetNodes) {
            TaskName   = taskName ?? String.Empty;
            TaskResult = result   ?? throw new ArgumentNullException(nameof(result));
        }

        public ParameterCodeModel TaskResult { get; }
        public string TaskName { get; }
        public string TaskNamePascalcase => TaskName.ToPascalcase();

        public static ExitTransitionCodeModel FromNode(ITaskNodeSymbol node) {

            var nodes = CallCodeModelBuilder.FromCalls(node.GetDistinctOutgoingCalls());
           
            // TODO Result
            var taskResult = new ParameterCodeModel("bool", "todo");
            
            return new ExitTransitionCodeModel(nodes.ToImmutableList(), taskResult, node.Name);
        }
    }
}