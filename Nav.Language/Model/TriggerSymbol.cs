namespace Pharmatechnik.Nav.Language {

    abstract class TriggerSymbol: Symbol, ITriggerSymbol {

        // ReSharper disable once NotNullMemberIsNotInitialized Transition wird im Ctor der Transition während der Initialisierung gesetzt 
        // In der "freien" Wildbahn" darf hingegen der Null Fall nicht auftreten
        protected TriggerSymbol(string name, Location location) : base(name, location) {
        }

        public ITransition Transition { get; set; }
        public abstract bool IsSignalTrigger { get; }
        public abstract bool IsSpontaneousTrigger { get; }
    }

    sealed partial class SignalTriggerSymbol : TriggerSymbol, ISignalTriggerSymbol {

        public SignalTriggerSymbol(string name, Location location) : base(name, location) {
        }

        public override bool IsSignalTrigger { get { return true; } }
        public override bool IsSpontaneousTrigger { get { return false; } }
    }

    sealed partial class SpontaneousTriggerSymbol : TriggerSymbol, ISpontaneousTriggerSymbol {

        public SpontaneousTriggerSymbol(Location location) : base(SpontaneousTriggerSyntax.Keyword, location) {
        }

        public override bool IsSignalTrigger { get { return false; } }
        public override bool IsSpontaneousTrigger { get { return true; } }
    }
}