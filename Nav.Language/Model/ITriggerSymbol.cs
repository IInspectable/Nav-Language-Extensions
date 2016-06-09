using JetBrains.Annotations;

namespace Pharmatechnik.Nav.Language {

    public interface ITriggerSymbol : ISymbol {

        [NotNull]
        ITransition Transition { get; }
        bool IsSignalTrigger { get; }
        bool IsSpontaneousTrigger { get; }

    }

    // F�r den isitor ist es g�nstiger, explizite Interfaces zu haben..
    public interface ISignalTriggerSymbol : ITriggerSymbol {
    }

    public interface ISpontaneousTriggerSymbol : ITriggerSymbol {
    }
}