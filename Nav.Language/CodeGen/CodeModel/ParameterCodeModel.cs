#region Using Directives

using System;
using System.Linq;
using System.Collections.Generic;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen {
    public class ParameterCodeModel : CodeModel {
        
        public ParameterCodeModel(string parameterType, string parameterName) {
            ParameterType = parameterType  ?? String.Empty;
            ParameterName = (parameterName ?? String.Empty).ToCamelcase();
        }

        public virtual string ParameterType { get; }
        public virtual string ParameterName { get; }

        public static IEnumerable<ParameterCodeModel> FromParameterSyntaxes(IEnumerable<ParameterSyntax> parameters) {
            if (parameters == null) {
                yield break;
            }

            string GetParameterName(string name, ref int index) {
                return String.IsNullOrEmpty(name) ? $"p{index++}" : name;
            }

            // TODO parameterName Fallback überprüfen
            int i = 1;
            foreach (var parameterSyntax in parameters) {
               yield return new ParameterCodeModel(
                    parameterType: parameterSyntax.Type?.ToString(),
                    parameterName: GetParameterName(parameterSyntax.Identifier.ToString(), ref i));
            }
        }

        public static IEnumerable<ParameterCodeModel> GetTaskBeginsAsParameter(IEnumerable<ITaskDeclarationSymbol> taskDeclarations) {
            return taskDeclarations.Select(GetTaskBeginAsParameter);
        }

        public static ParameterCodeModel GetTaskBeginAsParameter(ITaskDeclarationSymbol taskDeclaration) {

            // If a task is not implemented, there is no IBegin interface for it! - so the generated
            // code will not compile. Therefore, we generate IWFService instead. When the task IS
            // implemented one day, regeneration (necessary for constructor parameters anyway!)
            // will insert the correct types.
            var parameterType = CodeGenFacts.DefaultIwfsBaseType;

            if (!taskDeclaration.CodeNotImplemented) {
                var codeNamespace = String.IsNullOrEmpty(taskDeclaration.CodeNamespace) ? CodeGenFacts.UnknownNamespace : taskDeclaration.CodeNamespace;
                parameterType = $"{codeNamespace}.{CodeGenFacts.WflNamespaceSuffix}.{CodeGenFacts.BeginInterfacePrefix}{taskDeclaration.Name}{CodeGenFacts.WfsClassSuffix}";
            }

            var parameterName = taskDeclaration.Name; 

            return new ParameterCodeModel(parameterType: parameterType, parameterName: parameterName);
        }       
    }
}
