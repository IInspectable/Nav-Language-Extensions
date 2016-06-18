using JetBrains.Annotations;

namespace Pharmatechnik.Nav.Language {

    public interface IExitTransition: IEdge {

        [NotNull]
        ExitTransitionDefinitionSyntax Syntax { get; }

        ITaskDefinitionSymbol TaskDefinition { get; }

        [CanBeNull]
        IConnectionPointReferenceSymbol ConnectionPoint { get; }
    }
}