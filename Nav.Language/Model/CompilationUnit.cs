#region Using Directives

using System;
using System.Linq;
using System.Threading;
using System.Collections.Generic;

using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language {

    public sealed class CompilationUnit {

        internal CompilationUnit(CompilationUnitSyntax syntax,
                               IReadOnlyList<string> codeUsings,
                               IReadOnlySymbolCollection<ITaskDeclarationSymbol> taskDeclarations,
                               IReadOnlySymbolCollection<ITaskDefinitionSymbol> taskDefinitions,
                               IReadOnlySymbolCollection<IIncludeSymbol> includes,
                               IEnumerable<ISymbol> symbols,
                               IReadOnlyList<Diagnostic> diagnostics) {

            if(syntax == null) {
                throw new ArgumentNullException(nameof(syntax));
            }

            Syntax           = syntax;
            CodeUsings       = codeUsings       ?? new List<string>();
            TaskDeclarations = taskDeclarations ?? new SymbolCollection<ITaskDeclarationSymbol>();
            TaskDefinitions  = taskDefinitions  ?? new SymbolCollection<ITaskDefinitionSymbol>();
            Diagnostics      = diagnostics      ?? new List<Diagnostic>();
            Includes         = includes         ?? new SymbolCollection<IIncludeSymbol>();
            Symbols          = new SymbolList(symbols ?? Enumerable.Empty<IIncludeSymbol>());            
        }

        [NotNull]
        public CompilationUnitSyntax Syntax { get; }

        [NotNull]
        public string CodeNamespace {
            get { return Syntax.CodeNamespace?.ToString() ?? String.Empty; }
        }

        [NotNull]
        public IReadOnlyList<string> CodeUsings { get; }

        [NotNull]
        public IReadOnlySymbolCollection<IIncludeSymbol> Includes { get; }

        [NotNull]
        public IReadOnlySymbolCollection<ITaskDeclarationSymbol> TaskDeclarations { get; }

        [NotNull]
        public IReadOnlySymbolCollection<ITaskDefinitionSymbol> TaskDefinitions { get; }

        [NotNull]
        public SymbolList Symbols { get; }

        [NotNull]
        public IReadOnlyList<Diagnostic> Diagnostics { get; }
        
        public static CompilationUnit FromCompilationUnitSyntax(CompilationUnitSyntax syntax, CancellationToken cancellationToken = default(CancellationToken)) {
            return CompilationUnitBuilder.FromCompilationUnit(syntax, cancellationToken);
        }       
    }
}
