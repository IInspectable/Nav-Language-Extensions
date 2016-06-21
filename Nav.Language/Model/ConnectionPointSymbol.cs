namespace Pharmatechnik.Nav.Language {

    abstract class ConnectionPointSymbol : Symbol, IConnectionPointSymbol {

        protected ConnectionPointSymbol(ConnectionPointKind kind, string name, Location location, TaskDeclarationSymbol taskDeclaration) : base(name, location) {
            Kind = kind;
            TaskDeclaration = taskDeclaration;
        }

        public ConnectionPointKind Kind { get; }
        public ITaskDeclarationSymbol TaskDeclaration { get; }
    }

    sealed partial class InitConnectionPointSymbol : ConnectionPointSymbol, IInitConnectionPointSymbol {

        public InitConnectionPointSymbol(string name, Location location, InitNodeDeclarationSyntax syntax, TaskDeclarationSymbol taskDeclaration) 
            : base(ConnectionPointKind.Init, name, location, taskDeclaration) {
            Syntax = syntax;
        }

        public InitNodeDeclarationSyntax Syntax { get; }
    }

    sealed partial class ExitConnectionPointSymbol : ConnectionPointSymbol, IExitConnectionPointSymbol {

        public ExitConnectionPointSymbol(string name, Location location, ExitNodeDeclarationSyntax syntax, TaskDeclarationSymbol taskDeclaration) 
            : base(ConnectionPointKind.Exit, name, location, taskDeclaration) {
            Syntax = syntax;
        }

        public ExitNodeDeclarationSyntax Syntax { get; }
    }

    sealed partial class EndConnectionPointSymbol : ConnectionPointSymbol, IEndConnectionPointSymbol {

        public EndConnectionPointSymbol(string name, Location location, EndNodeDeclarationSyntax syntax, TaskDeclarationSymbol taskDeclaration)
            : base(ConnectionPointKind.End, name, location, taskDeclaration) {
            Syntax = syntax;
        }

        public EndNodeDeclarationSyntax Syntax { get; }
    }
}