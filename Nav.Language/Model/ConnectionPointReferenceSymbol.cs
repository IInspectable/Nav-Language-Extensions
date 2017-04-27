using JetBrains.Annotations;

namespace Pharmatechnik.Nav.Language {

    sealed partial class ConnectionPointReferenceSymbol: Symbol, IConnectionPointReferenceSymbol {

        // ReSharper disable once NotNullMemberIsNotInitialized ExitTransition wird definitiv im Ctor der ExitTransition gesetzt
        public ConnectionPointReferenceSymbol(string name, [NotNull] Location location, IConnectionPointSymbol connectionPoint) 
            : base(name, location) {
            Declaration = connectionPoint;
        }

        public IConnectionPointSymbol Declaration { get; }
        public IExitTransition ExitTransition { get; internal set; }
    }
}