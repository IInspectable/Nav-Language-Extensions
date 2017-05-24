#region Using Directives

using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language {

    sealed class CodeParameter: ICodeParameter {

        CodeParameter(string parameterName, string paramterType, [NotNull] Location location) {
            ParameterType = paramterType  ?? string.Empty;
            ParameterName = parameterName ?? string.Empty;
            Location      = location;
        }
        
        public string ParameterName { get; }
        [NotNull]
        public Location Location { get; }
        public string ParameterType { get; }

        [CanBeNull]
        public static ICodeParameter FromResultDeclaration([CanBeNull]CodeResultDeclarationSyntax codeResult) {

            if (codeResult?.Result == null || codeResult.Result?.Type==null || codeResult.Result.Identifier.IsMissing) {
                return null;
            }

            return new CodeParameter(
                parameterName: codeResult.Result.Identifier.ToString(), 
                paramterType : codeResult.Result.Type.ToString(), 
                location     : codeResult.GetLocation());
        }
    }
}