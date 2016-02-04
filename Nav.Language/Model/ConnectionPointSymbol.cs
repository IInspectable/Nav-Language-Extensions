namespace Pharmatechnik.Nav.Language {

    abstract class ConnectionPointSymbol : Symbol, IConnectionPointSymbol {

        protected ConnectionPointSymbol(ConnectionPointKind kind, string name, Location location): base(name, location) {
            Kind = kind;
        }

        public ConnectionPointKind Kind { get; }       
    }

    sealed partial class InitConnectionPointSymbol : ConnectionPointSymbol, IInitConnectionPointSymbol {

        public InitConnectionPointSymbol(string name, Location location, InitNodeDeclarationSyntax syntax) : base(ConnectionPointKind.Init, name, location) {
            Syntax = syntax;
        }

        public InitNodeDeclarationSyntax Syntax { get; }
    }

    sealed partial class ExitConnectionPointSymbol : ConnectionPointSymbol, IExitConnectionPointSymbol {

        public ExitConnectionPointSymbol(string name, Location location, ExitNodeDeclarationSyntax syntax) : base(ConnectionPointKind.Exit, name, location) {
            Syntax = syntax;
        }

        public ExitNodeDeclarationSyntax Syntax { get; }
    }

    sealed partial class EndConnectionPointSymbol : ConnectionPointSymbol, IEndConnectionPointSymbol {

        public EndConnectionPointSymbol(string name, Location location, EndNodeDeclarationSyntax syntax) : base(ConnectionPointKind.End, name, location) {
            Syntax = syntax;
        }

        public EndNodeDeclarationSyntax Syntax { get; }
    }
}