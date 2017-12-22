using JetBrains.Annotations;

namespace Pharmatechnik.Nav.Language {

    public interface ITriggerSymbol : ISymbol {

        [NotNull]
        ITransition Transition { get; }
        bool IsSignalTrigger { get; }
        bool IsSpontaneousTrigger { get; }
    }

    // F�r den visitor ist es g�nstiger, explizite Interfaces zu haben..
    public interface ISignalTriggerSymbol : ITriggerSymbol {
        [NotNull]
        IdentifierOrStringSyntax Syntax { get; }

        [NotNull]
        new ITriggerTransition Transition { get; }
    }

    public interface ISpontaneousTriggerSymbol : ITriggerSymbol {
        [NotNull]
        SpontaneousTriggerSyntax Syntax { get; }
    }
}