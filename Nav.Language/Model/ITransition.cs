using JetBrains.Annotations;

namespace Pharmatechnik.Nav.Language {

    public interface ITransition: IEdge {

        [NotNull]
        TransitionDefinitionSyntax Syntax { get; }              
    }

    public interface IInitTransition: ITransition {
        IInitNodeReferenceSymbol InitNodeReference{get;}
    }

    public interface ITriggerTransition: ITransition {

        // TODO GuiNodeReference einf�hren
        [NotNull]
        IReadOnlySymbolCollection<ITriggerSymbol> Triggers { get; }
    }
}