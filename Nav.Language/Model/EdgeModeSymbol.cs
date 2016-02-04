namespace Pharmatechnik.Nav.Language {

    sealed partial class EdgeModeSymbol : Symbol, IEdgeModeSymbol {
        
        public EdgeModeSymbol(string name, Location location, EdgeMode edgeMode) : base(name, location) {
            EdgeMode = edgeMode;
        }

        public EdgeMode EdgeMode { get;}
    }
}