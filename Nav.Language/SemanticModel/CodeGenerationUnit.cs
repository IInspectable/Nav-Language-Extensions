#region Using Directives

using System;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Immutable;

using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language {

    public sealed class CodeGenerationUnit {

        // TODO IReadOnlyList mit Immutable ersetzen
        internal CodeGenerationUnit(CodeGenerationUnitSyntax syntax,
                                    ImmutableList<string> codeUsings,
                                    IReadOnlySymbolCollection<ITaskDeclarationSymbol> taskDeclarations,
                                    IReadOnlySymbolCollection<ITaskDefinitionSymbol> taskDefinitions,
                                    IReadOnlySymbolCollection<IIncludeSymbol> includes,
                                    IEnumerable<ISymbol> symbols,
                                    ImmutableList<Diagnostic> diagnostics) {

            Syntax           = syntax           ?? throw new ArgumentNullException(nameof(syntax));
            CodeUsings       = codeUsings       ?? ImmutableList.Create<string>();
            TaskDeclarations = taskDeclarations ?? new SymbolCollection<ITaskDeclarationSymbol>();
            TaskDefinitions  = taskDefinitions  ?? new SymbolCollection<ITaskDefinitionSymbol>();
            Diagnostics      = diagnostics      ?? ImmutableList.Create<Diagnostic>();
            Includes         = includes         ?? new SymbolCollection<IIncludeSymbol>();
            Symbols          = new SymbolList(symbols ?? Enumerable.Empty<IIncludeSymbol>());            
        }

        public CodeGenerationUnit WithDiagnostics(IList<Diagnostic> diagnostics) {
            return new CodeGenerationUnit(
                Syntax, 
                CodeUsings, 
                TaskDeclarations, 
                TaskDefinitions, 
                Includes, 
                Symbols,
                diagnostics.ToImmutableList());
        }

        [NotNull]
        public CodeGenerationUnitSyntax Syntax { get; }

        [NotNull]
        public string CodeNamespace => Syntax.CodeNamespace?.ToString() ?? String.Empty;

        [NotNull]
        public ImmutableList<string> CodeUsings { get; }

        [NotNull]
        public IReadOnlySymbolCollection<IIncludeSymbol> Includes { get; }

        [NotNull]
        public IReadOnlySymbolCollection<ITaskDeclarationSymbol> TaskDeclarations { get; }

        [NotNull]
        public IReadOnlySymbolCollection<ITaskDefinitionSymbol> TaskDefinitions { get; }

        [NotNull]
        public SymbolList Symbols { get; }

        [NotNull]
        public ImmutableList<Diagnostic> Diagnostics { get; }

        [NotNull]
        public static CodeGenerationUnit FromCodeGenerationUnitSyntax(CodeGenerationUnitSyntax syntax, CancellationToken cancellationToken = default, ISyntaxProvider syntaxProvider=null) {
            return CodeGenerationUnitBuilder.FromCodeGenerationUnitSyntax(syntax, cancellationToken, syntaxProvider);
        }       
    }
}
