namespace Pharmatechnik.Nav.Language {

    public enum ConnectionPointKind {
        Init,
        Exit,
        End
    }

    public interface IConnectionPointSymbol: ISymbol {
        ConnectionPointKind Kind { get; }
    }

    public interface IInitConnectionPointSymbol : IConnectionPointSymbol {
        InitNodeDeclarationSyntax Syntax { get; }
    }

    public interface IExitConnectionPointSymbol : IConnectionPointSymbol {
        ExitNodeDeclarationSyntax Syntax { get; }
    }

    public interface IEndConnectionPointSymbol : IConnectionPointSymbol {
        EndNodeDeclarationSyntax Syntax { get; }
    }    
}