#region Using Directives

using System;
using System.Collections.Generic;
using System.Collections.Immutable;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen {

    sealed class InitTransitionCodeModel : TransitionCodeModel {

        InitTransitionCodeModel(ImmutableList<ParameterCodeModel> parameter, ImmutableList<CallCodeModel> targetNodes) 
            :base(targetNodes){

            Parameter = parameter ?? throw new ArgumentNullException(nameof(parameter));           
        }
        
        internal static InitTransitionCodeModel FromInitNode(IInitNodeSymbol initNodeSymbol, TaskCodeModel taskCodeModel) {

            if (initNodeSymbol == null) {
                throw new ArgumentNullException(nameof(initNodeSymbol));
            }
            if (taskCodeModel == null) {
                throw new ArgumentNullException(nameof(taskCodeModel));
            }
            
            var parameter = ParameterCodeModel.FromParameterSyntaxes(initNodeSymbol.Syntax.CodeParamsDeclaration?.ParameterList);
            var nodes     = CallCodeModelBuilder.FromCalls(initNodeSymbol.GetDistinctOutgoingCalls());

            return new InitTransitionCodeModel(
                parameter  : parameter.ToImmutableList(), 
                targetNodes: nodes.ToImmutableList());
        }

        public ImmutableList<ParameterCodeModel> Parameter { get; }
    }
}