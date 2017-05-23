#region Using Directives

using System;
using System.Collections.Immutable;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen {

    sealed class InitTransitionCodeModel : TransitionCodeModel {

        InitTransitionCodeModel(ImmutableList<ParameterCodeModel> parameter, ImmutableList<Call> reachableCalls) 
            :base(reachableCalls) {

            Parameter = parameter ?? throw new ArgumentNullException(nameof(parameter));           
        }
        
        internal static InitTransitionCodeModel FromInitTransition(ITransition initTransition, TaskCodeModel taskCodeModel) {

            var initNode = initTransition?.Source?.Declaration as IInitNodeSymbol;
            if (initNode == null) {
                throw new ArgumentException("Init transition expected");
            }
            if (taskCodeModel == null) {
                throw new ArgumentNullException(nameof(taskCodeModel));
            }
            
            var parameter = ParameterCodeModel.FromParameterSyntaxes(initNode.Syntax.CodeParamsDeclaration?.ParameterList);

            return new InitTransitionCodeModel(
                parameter     : parameter.ToImmutableList(),
                reachableCalls: initTransition.GetReachableCalls().ToImmutableList());
        }

        public ImmutableList<ParameterCodeModel> Parameter { get; }
    }
}