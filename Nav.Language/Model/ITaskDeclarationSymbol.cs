#region Using Directives

using System.Collections.Generic;
using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language {

    public enum TaskDeclarationOrigin {
        TaskDeclaration,
        TaskDefinition
    }
    public interface ITaskDeclarationSymbol : ISymbol {

        [NotNull]
        IReadOnlySymbolCollection<IConnectionPointSymbol> ConnectionPoints { get; }

        IReadOnlySymbolCollection<IConnectionPointSymbol> Inits();
        IReadOnlySymbolCollection<IConnectionPointSymbol> Exits();
        IReadOnlySymbolCollection<IConnectionPointSymbol> Ends();
        
        [NotNull]
        IReadOnlyList<ITaskNodeSymbol> References { get; }

        bool IsIncluded { get; }
        TaskDeclarationOrigin Origin { get; }
    }
}