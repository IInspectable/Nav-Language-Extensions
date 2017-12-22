using JetBrains.Annotations;

namespace Pharmatechnik.Nav.Language {

    public interface IExitTransition: IEdge {

        [NotNull]
        ExitTransitionDefinitionSyntax Syntax { get; }
        
        [CanBeNull]
        IConnectionPointReferenceSymbol ConnectionPointReference { get; }
    }
}