#region Using Directives

using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language {

    sealed partial class CodeParameterSymbol: Symbol, ICodeParameterSymbol {

        CodeParameterSymbol(string name, string paramterType, [NotNull] Location location) : base(name, location) {
            ParamterType = paramterType ?? string.Empty;
        }
        
        public string ParamterName => Name.ToCamelcase();
        public string ParamterType { get; }

        [CanBeNull]
        public static ICodeParameterSymbol FromResultDeclaration([CanBeNull]CodeResultDeclarationSyntax codeResult) {

            if (codeResult?.Result == null || codeResult.Result?.Type==null || codeResult.Result.Identifier.IsMissing) {
                return null;
            }

            return new CodeParameterSymbol(
                name        : codeResult.Result.Identifier.ToString(), 
                paramterType: codeResult.Result.Type.ToString(), 
                location    : codeResult.GetLocation());
        }
    }
}