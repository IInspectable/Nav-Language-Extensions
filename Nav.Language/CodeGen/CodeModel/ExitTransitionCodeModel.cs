﻿#region Using Directives

using System;
using System.Linq;
using System.Collections.Immutable;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen {

    class ExitTransitionCodeModel : TransitionCodeModel {

        public ExitTransitionCodeModel(ImmutableList<CallCodeModel> calls, 
                                       ParameterCodeModel result, string taskName, 
                                       ImmutableList<ParameterCodeModel> taskBegins, 
                                       ImmutableList<FieldCodeModel> taskBeginFields)
            :base (calls) {
            TaskBegins      = taskBegins       ?? throw new ArgumentNullException(nameof(taskBegins));
            TaskBeginFields = taskBeginFields  ?? throw new ArgumentNullException(nameof(taskBeginFields));
            TaskName        = taskName         ?? String.Empty;
            TaskResult      = result           ?? throw new ArgumentNullException(nameof(result));
        }

        public ParameterCodeModel TaskResult { get; }
        public string TaskName { get; }
        public string TaskNamePascalcase => TaskName.ToPascalcase();
        public ImmutableList<ParameterCodeModel> TaskBegins { get; }
        public ImmutableList<FieldCodeModel> TaskBeginFields { get; }

        public static ExitTransitionCodeModel FromNode(ITaskNodeSymbol node) {

            var reachableCalls = node.GetDistinctOutgoingCalls().ToList();
            var calls = CallCodeModelBuilder.FromCalls(reachableCalls);

            var reachableNodes   = reachableCalls.Select(c => c.Node).ToList();
            
            var taskBegins      = GetTaskBegins(reachableNodes);
            var taskBeginFields = GetTaskBeginFields(reachableNodes);

            // TODO Result
            var taskResult = new ParameterCodeModel("bool", "todo");
            
            return new ExitTransitionCodeModel(
                calls           : calls.ToImmutableList(), 
                result          : taskResult, 
                taskName        : node.Name, 
                taskBegins      : taskBegins.ToImmutableList(), 
                taskBeginFields : taskBeginFields.ToImmutableList());
        }
    }
}