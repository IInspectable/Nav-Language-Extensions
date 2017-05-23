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

            string GetParameterName(string name, ref int i)
            {
                return String.IsNullOrEmpty(name) ? $"p{i++}" : name;
            }

            var parameter = new List<ParameterCodeModel>();
            var paramterList = initNodeSymbol.Syntax.CodeParamsDeclaration?.ParameterList;
            if (paramterList != null) {
                // TODO parameterName Fallback überprüfen
                int i = 1;
                foreach (var parameterSyntax in paramterList) {
                    parameter.Add(new ParameterCodeModel(
                        parameterType: parameterSyntax.Type?.ToString(),
                        parameterName: GetParameterName(parameterSyntax.Identifier.ToString(), ref i)));
                }
            }

            // TODO Transitions

            var nodes = CallCodeModelBuilder.FromCalls(initNodeSymbol.GetDistinctOutgoingCalls());

            return new InitTransitionCodeModel(
                parameter  : parameter.ToImmutableList(), 
                targetNodes: nodes.ToImmutableList());
        }

        public ImmutableList<ParameterCodeModel> Parameter { get; }
    }
}