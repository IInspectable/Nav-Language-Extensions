#region Using Directives

using System;
using System.Collections.Immutable;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen {

    sealed class InitTransitionCodeModel : TransitionCodeModel {

        InitTransitionCodeModel(ImmutableList<ParameterCodeModel> parameter, ImmutableList<Call> reachableCalls,
                                bool generateAbstractMethod) 
            :base(reachableCalls) {

            Parameter              = parameter ?? throw new ArgumentNullException(nameof(parameter));
            GenerateAbstractMethod = generateAbstractMethod;
        }
        
        public bool GenerateAbstractMethod { get; }
        public string BeginMethodName      => CodeGenFacts.BeginMethodPrefix;
        public string BeginLogicMethodName => $"{CodeGenFacts.BeginMethodPrefix}{CodeGenFacts.LogicMethodSuffix}";

        public ImmutableList<ParameterCodeModel> Parameter { get; }

        internal static InitTransitionCodeModel FromInitTransition(ITransition initTransition, TaskCodeInfo taskCodeInfo) {

            var initNode = initTransition?.Source?.Declaration as IInitNodeSymbol;
            if (initNode == null) {
                throw new ArgumentException("Init transition expected");
            }
            if (taskCodeInfo == null) {
                throw new ArgumentNullException(nameof(taskCodeInfo));
            }
            
            var parameter = ParameterCodeModel.FromParameterSyntaxes(initNode.Syntax.CodeParamsDeclaration?.ParameterList);

            return new InitTransitionCodeModel(
                parameter             : parameter.ToImmutableList(),
                reachableCalls        : initTransition.GetReachableCalls().ToImmutableList(),
                generateAbstractMethod: initNode.CodeGenerateAbstractMethod());
        }        
    }
}