#region Using Directives

using System;
using System.Collections.Generic;

#endregion

namespace Pharmatechnik.Nav.Language {

    sealed partial class IncludeSymbol: Symbol, IIncludeSymbol {

        public IncludeSymbol(string fileName,
                             Location location,
                             Location fileLocation,
                             IncludeDirectiveSyntax syntax,
                             IReadOnlyList<Diagnostic> diagnostics,
                             SymbolCollection<TaskDeclarationSymbol> taskDeklarations)
            : base(fileName.ToLowerInvariant(), location) {

            FileName         = fileName;
            FileLocation     = fileLocation     ?? throw new ArgumentNullException(nameof(fileLocation));
            Syntax           = syntax           ?? throw new ArgumentNullException(nameof(syntax));
            Diagnostics      = diagnostics      ?? new List<Diagnostic>();
            TaskDeklarations = taskDeklarations ?? new SymbolCollection<TaskDeclarationSymbol>();
        }

        public override SyntaxTree SyntaxTree => Syntax.SyntaxTree;

        public string                                  FileName         { get; }
        public Location                                FileLocation     { get; }
        public IncludeDirectiveSyntax                  Syntax           { get; }
        public IReadOnlyList<Diagnostic>               Diagnostics      { get; }
        public SymbolCollection<TaskDeclarationSymbol> TaskDeklarations { get; }

        IReadOnlySymbolCollection<ITaskDeclarationSymbol> IIncludeSymbol.TaskDeklarations => TaskDeklarations;

    }

}