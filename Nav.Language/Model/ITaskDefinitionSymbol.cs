#region Using Directives

using System.Collections.Generic;
using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language {

    public interface ITaskDefinitionSymbol : ISymbol {

        [NotNull]
        TaskDefinitionSyntax Syntax { get; }

        [CanBeNull]
        ITaskDeclarationSymbol AsTaskDeclaration { get; }

        [NotNull]
        IReadOnlySymbolCollection<INodeSymbol> NodeDeclarations { get; }

        [NotNull]
        IReadOnlyList<ITransition> Transitions { get; }

        [NotNull]
        IReadOnlyList<IExitTransition> ExitTransitions { get; }
    }
}