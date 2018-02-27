using JetBrains.Annotations;

namespace Pharmatechnik.Nav.Language {

    public interface ITransition: IEdge {

        [NotNull]
        TransitionDefinitionSyntax Syntax { get; }

    }

    public interface IInitTransition: ITransition {

        IInitNodeReferenceSymbol InitNodeReference { get; }

    }

    public interface ITriggerTransition: ITransition {

        IGuiNodeReferenceSymbol GuiNodeReference { get; }

        [NotNull]
        IReadOnlySymbolCollection<ITriggerSymbol> Triggers { get; }

    }

    public interface IChoiceTransition: ITransition {

        IChoiceNodeReferenceSymbol ChoiceNodeReference { get; }

    }

}