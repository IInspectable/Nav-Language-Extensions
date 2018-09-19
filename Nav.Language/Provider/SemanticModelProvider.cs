#region Using Directives

using System;

#endregion

namespace Pharmatechnik.Nav.Language {

    public class SemanticModelProvider: ISemanticModelProvider {

        private readonly ISyntaxProvider _syntaxProvider;

        public SemanticModelProvider(ISyntaxProvider syntaxProvider) {
            _syntaxProvider = syntaxProvider ?? throw new ArgumentNullException(nameof(syntaxProvider));
        }

        public static readonly ISemanticModelProvider Default = new SemanticModelProvider(SyntaxProvider.Default);

        public CodeGenerationUnit GetSemanticModel(CodeGenerationUnitSyntax syntax) {
            return CodeGenerationUnit.FromCodeGenerationUnitSyntax(syntax, syntaxProvider: _syntaxProvider);
        }

        public virtual void Dispose() {
        }

    }

}