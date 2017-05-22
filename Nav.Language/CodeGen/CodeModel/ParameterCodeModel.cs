#region Using Directives

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen {
    public class ParameterCodeModel : CodeModel {
        
        public ParameterCodeModel(string parameterType, string parameterName) {
            ParameterType = parameterType ?? String.Empty;
            ParameterName = parameterName ?? String.Empty;
        }

        [NotNull]
        public string ParameterType { get; }

        [NotNull]
        public string ParameterName { get; }


        public static ParameterCodeModel FromTaskDeclaration(ITaskDeclarationSymbol taskDeclaration) {

            // If a task is not implemented, there is no IBegin interface for it! - so the generated
            // code will not compile. Therefore, we generate IWFService instead. When the task IS
            // implemented one day, regeneration (necessary for constructor parameters anyway!)
            // will insert the correct types.
            var parameterType = CodeGenFacts.DefaultIwfsBaseType;

            if (!taskDeclaration.CodeNotImplemented) {
                var codeNamespace = String.IsNullOrEmpty(taskDeclaration.CodeNamespace) ? CodeGenFacts.UnknownNamespace : taskDeclaration.CodeNamespace;
                parameterType = $"{codeNamespace}.{CodeGenFacts.WflNamespaceSuffix}.{CodeGenFacts.BeginInterfacePrefix}{taskDeclaration.Name}{CodeGenFacts.WfsClassSuffix}";
            }

            var parameterName = taskDeclaration.Name.ToCamelcase();

            return new ParameterCodeModel(parameterType: parameterType, parameterName: parameterName);
        }

        public static ImmutableList<ParameterCodeModel> FromParameterSyntax(IEnumerable<ParameterSyntax> parameters) {
            // TODO Vermutlich darf nur nach Name sortiert werden
            return parameters.Select(FromParameterSyntax).OrderBy(p => p.ParameterType.Length + p.ParameterName.Length).ToImmutableList();
        }

        static ParameterCodeModel FromParameterSyntax(ParameterSyntax parameter) {
            return new ParameterCodeModel(parameterType: parameter.Type?.ToString(), parameterName: parameter.Identifier.ToString());
        }
    }
}
