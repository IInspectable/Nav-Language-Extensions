using JetBrains.Annotations;

namespace Pharmatechnik.Nav.Language;

public interface IConcatableEdge: IEdge {

    [CanBeNull]
    IConcatTransition ConcatTransition { get; }

}

public interface ITransition: IConcatableEdge {

    [NotNull]
    TransitionDefinitionSyntax Syntax { get; }

}

public interface IInitTransition: ITransition {

    [CanBeNull]
    IInitNodeReferenceSymbol InitNodeSourceReference { get; }

}

public interface ITriggerTransition: ITransition {

    [CanBeNull]
    IGuiNodeReferenceSymbol GuiNodeSourceReference { get; }

    [NotNull]
    IReadOnlySymbolCollection<ITriggerSymbol> Triggers { get; }

}

public interface IChoiceTransition: ITransition {

    [CanBeNull]
    IChoiceNodeReferenceSymbol ChoiceNodeSourceReference { get; }

}

public interface IExitTransition: IConcatableEdge {

    [NotNull]
    ExitTransitionDefinitionSyntax Syntax { get; }

    [CanBeNull]
    ITaskNodeReferenceSymbol TaskNodeSourceReference { get; }

    [CanBeNull]
    IExitConnectionPointReferenceSymbol ExitConnectionPointReference { get; }

}

public interface IConcatTransition: IEdge {

    [NotNull]
    ConcatTransitionSyntax Syntax { get; }

}