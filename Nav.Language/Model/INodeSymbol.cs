using System.Collections.Generic;
using JetBrains.Annotations;

namespace Pharmatechnik.Nav.Language {

    public interface INodeSymbol : ISymbol {

        [NotNull]
        NodeDeclarationSyntax Syntax { get; }

        [NotNull]
        ITaskDefinitionSymbol ContainingTask { get; }

        [NotNull]
        IReadOnlyList<INodeReferenceSymbol> References { get; }

        IEnumerable<ISymbol> SymbolsAndSelf();
    }

    public interface IInitNodeSymbol : INodeSymbol {

        [NotNull]
        new InitNodeDeclarationSyntax Syntax { get; }

        [CanBeNull]
        IInitNodeAliasSymbol Alias { get; }

        [NotNull]
        IReadOnlyList<ITransition> Outgoings { get; }
    }

    public interface IExitNodeSymbol : INodeSymbol {

        [NotNull]
        new ExitNodeDeclarationSyntax Syntax { get; }

        [NotNull]
        IReadOnlyList<IEdge> Incomings { get; }
    }

    public interface IEndNodeSymbol : INodeSymbol {
        [NotNull]
        new EndNodeDeclarationSyntax Syntax { get; }

        [NotNull]
        IReadOnlyList<IEdge> Incomings { get; }
    }

    public interface IChoiceNodeSymbol : INodeSymbol {

        [NotNull]
        new ChoiceNodeDeclarationSyntax Syntax { get; }

        [NotNull]
        IReadOnlyList<IEdge> Incomings { get; }

        [NotNull]
        IReadOnlyList<ITransition> Outgoings { get; }
    }

    public interface IGuiNodeSymbol : INodeSymbol {

        [NotNull]
        IReadOnlyList<IEdge> Incomings { get; }

        [NotNull]
        IReadOnlyList<ITransition> Outgoings { get; }

    }

    public interface IViewNodeSymbol : IGuiNodeSymbol {

        [NotNull]
        new ViewNodeDeclarationSyntax Syntax { get; }       
    }

    public interface IDialogNodeSymbol : IGuiNodeSymbol {

        [NotNull]
        new DialogNodeDeclarationSyntax Syntax { get; }
    }

    public interface ITaskNodeSymbol : INodeSymbol {

        [NotNull]
        new TaskNodeDeclarationSyntax Syntax { get; }

        [CanBeNull]
        ITaskDeclarationSymbol Declaration { get; }

        [CanBeNull]
        ITaskNodeAliasSymbol Alias { get; }

        [NotNull]
        IReadOnlyList<IEdge> Incomings { get; }

        [NotNull]
        IReadOnlyList<IExitTransition> Outgoings { get; }
    }
}