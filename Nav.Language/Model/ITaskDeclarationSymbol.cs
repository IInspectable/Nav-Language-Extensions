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

        /// <summary>
        /// Ist nur dann null, wenn IsIncluded true, da wir keine Syntaxbäume von anderen nav-Dateien 
        /// im Speicher halten wollen.
        /// </summary>
        [CanBeNull]
        MemberDeclarationSyntax Syntax { get; }

        /// <summary>
        /// Ist nur dann null, wenn IsIncluded true, da wir keine Syntaxbäume von anderen nav-Dateien 
        /// im Speicher halten wollen.
        /// </summary>
        [CanBeNull]
        CodeGenerationUnit CodeGenerationUnit { get; }

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