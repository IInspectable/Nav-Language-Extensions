namespace Pharmatechnik.Nav.Language {

    sealed partial class ConnectionPointReferenceSymbol: Symbol, IConnectionPointReferenceSymbol {

        public IConnectionPointSymbol Declaration { get;}

        public ConnectionPointReferenceSymbol(string name, Location location, IConnectionPointSymbol connectionPoint) 
            : base(name, location) {
            Declaration = connectionPoint;
        }
    }
}