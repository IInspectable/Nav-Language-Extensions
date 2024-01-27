#nullable enable

using System;
using System.Collections.Immutable;

namespace Pharmatechnik.Nav.Language.CodeGen;

sealed class InitTransitionCodeModel: TransitionCodeModel {

    InitTransitionCodeModel(TaskCodeInfo containingTask,
                            ImmutableList<ParameterCodeModel> parameter, ImmutableList<Call> reachableCalls,
                            bool generateAbstractMethod,
                            string nodeName,
                            int index)
        : base(containingTask, reachableCalls) {

        Parameter              = parameter ?? throw new ArgumentNullException(nameof(parameter));
        GenerateAbstractMethod = generateAbstractMethod;
        NodeName               = nodeName ?? String.Empty;
        Index                  = index;

    }

    public bool   GenerateAbstractMethod { get; }
    public string NodeName               { get; }
    public int    Index                  { get; }

    public override string GetCallContextClassName() => $"{CodeGenFacts.BeginMethodPrefix}{CodeGenFacts.CallContextClassSuffix}" + (Index > 0 ? "${index}" : "");

    public ImmutableList<ParameterCodeModel> Parameter { get; }

    internal static InitTransitionCodeModel FromInitTransition(TaskCodeInfo containingTask, IInitNodeSymbol initNode, TaskCodeInfo taskCodeInfo, int index) {
        if (initNode == null) {
            throw new ArgumentNullException(nameof(initNode));
        }

        if (taskCodeInfo == null) {
            throw new ArgumentNullException(nameof(taskCodeInfo));
        }

        var parameter = ParameterCodeModel.FromParameterSyntaxes(initNode.Syntax.CodeParamsDeclaration?.ParameterList);

        return new InitTransitionCodeModel(
            containingTask        :containingTask,
            parameter             : parameter.ToImmutableList(),
            reachableCalls        : initNode.GetReachableCalls().ToImmutableList(),
            generateAbstractMethod: initNode.CodeGenerateAbstractMethod(),
            nodeName              : initNode.Name,
            index                 : index);
    }

}