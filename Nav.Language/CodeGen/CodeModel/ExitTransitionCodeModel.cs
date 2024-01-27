#nullable enable

#region Using Directives

using System;
using System.Collections.Immutable;

using Pharmatechnik.Nav.Language.Text;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen; 

class ExitTransitionCodeModel: TransitionCodeModel {

    public ExitTransitionCodeModel(TaskCodeInfo containingTask,
                                   ImmutableList<Call> calls,
                                   ParameterCodeModel taskResult, 
                                   bool generateAbstractMethod, 
                                   string? nodeName)
        : base(containingTask, calls) {

        TaskResult             = taskResult ?? throw new ArgumentNullException(nameof(taskResult));
        GenerateAbstractMethod = generateAbstractMethod;
        NodeName               = nodeName ?? String.Empty;
    }

    public ParameterCodeModel TaskResult             { get; }
    public bool               GenerateAbstractMethod { get; }
    public string             NodeName               { get; }
    public string             NodeNamePascalcase     => NodeName.ToPascalcase();

    public override string GetCallContextClassName() => $"{CodeGenFacts.ExitMethodPrefix}{NodeNamePascalcase}{CodeGenFacts.CallContextClassSuffix}";

    public static ExitTransitionCodeModel FromTaskNode(TaskCodeInfo containingTask, ITaskNodeSymbol taskNode) {

        if (taskNode == null) {
            throw new ArgumentNullException(nameof(taskNode));
        }

        var reachableCalls = taskNode.GetReachableCalls();
        var taskResult     = ParameterCodeModel.TaskResult(taskNode.Declaration);

        return new ExitTransitionCodeModel(
            containingTask        : containingTask,
            calls                 : reachableCalls.ToImmutableList(),
            taskResult            : taskResult,
            generateAbstractMethod: taskNode.CodeGenerateAbstractMethod(),
            nodeName              : taskNode.Name);
    }

}