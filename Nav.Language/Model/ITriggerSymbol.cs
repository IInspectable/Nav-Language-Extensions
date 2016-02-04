namespace Pharmatechnik.Nav.Language {

    public interface ITriggerSymbol : ISymbol {

        bool IsSignalTrigger { get; }
        bool IsSpontaneousTrigger { get; }

    }

    // Für den isitor ist es günstiger, explizite Interfaces zu haben..
    public interface ISignalTriggerSymbol : ITriggerSymbol {
    }

    public interface ISpontaneousTriggerSymbol : ITriggerSymbol {
    }
}