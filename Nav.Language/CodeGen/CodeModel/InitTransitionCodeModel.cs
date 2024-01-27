#nullable enable

using System;
using System.Collections.Immutable;

using Pharmatechnik.Nav.Language.Text;

namespace Pharmatechnik.Nav.Language.CodeGen;

sealed class InitTransitionCodeModel: TransitionCodeModel {

    InitTransitionCodeModel(TaskCodeInfo containingTask,
                            ImmutableList<ParameterCodeModel> parameter, ImmutableList<Call> reachableCalls,
                            bool generateAbstractMethod,
                            string nodeName)
        : base(containingTask, reachableCalls) {

        Parameter              = parameter ?? throw new ArgumentNullException(nameof(parameter));
        GenerateAbstractMethod = generateAbstractMethod;
        // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract
        NodeName               = nodeName ?? String.Empty;

    }

    public bool   GenerateAbstractMethod { get; }
    public string NodeName               { get; }

    public override string GetCallContextClassName() => $"{NodeName.ToPascalcase()}{CodeGenFacts.CallContextClassSuffix}";

    public ImmutableList<ParameterCodeModel> Parameter { get; }

    internal static InitTransitionCodeModel FromInitTransition(TaskCodeInfo containingTask, IInitNodeSymbol initNode) {
        if (initNode == null) {
            throw new ArgumentNullException(nameof(initNode));
        }

        if (containingTask == null) {
            throw new ArgumentNullException(nameof(containingTask));
        }

        var parameter = ParameterCodeModel.FromParameterSyntaxes(initNode.Syntax.CodeParamsDeclaration?.ParameterList);

        return new InitTransitionCodeModel(
            containingTask        : containingTask,
            parameter             : parameter.ToImmutableList(),
            reachableCalls        : initNode.GetReachableCalls().ToImmutableList(),
            generateAbstractMethod: initNode.CodeGenerateAbstractMethod(),
            nodeName              : initNode.Name);
    }

}