#region Using Directives

using System.Collections.Generic;
using System.Linq;

#endregion

namespace Pharmatechnik.Nav.Language {

    sealed partial class TaskDeclarationSymbol : Symbol, ITaskDeclarationSymbol {

        public TaskDeclarationSymbol(string name, Location location, TaskDeclarationOrigin origin, bool isIncluded): base(name, location) {
            Origin           = origin;
            IsIncluded       = isIncluded;
            References       = new List<ITaskNodeSymbol>();
            ConnectionPoints = new SymbolCollection<ConnectionPointSymbol>();
        }

        public SymbolCollection<ConnectionPointSymbol> ConnectionPoints { get; }
        public List<ITaskNodeSymbol> References { get; }


        IReadOnlySymbolCollection<IConnectionPointSymbol> ITaskDeclarationSymbol.ConnectionPoints {
            get { return ConnectionPoints; }
        }        

        IReadOnlySymbolCollection<IConnectionPointSymbol> ITaskDeclarationSymbol.Inits() {
            return new SymbolCollection<IConnectionPointSymbol>(ConnectionPoints.Where(cp => cp.Kind == ConnectionPointKind.Init));
        }

        IReadOnlySymbolCollection<IConnectionPointSymbol> ITaskDeclarationSymbol.Exits() {
            return new SymbolCollection<IConnectionPointSymbol>(ConnectionPoints.Where(cp => cp.Kind == ConnectionPointKind.Exit)); 
        }

        IReadOnlySymbolCollection<IConnectionPointSymbol> ITaskDeclarationSymbol.Ends() {
            return new SymbolCollection<IConnectionPointSymbol>(ConnectionPoints.Where(cp => cp.Kind == ConnectionPointKind.End));
        }

        IReadOnlyList<ITaskNodeSymbol> ITaskDeclarationSymbol.References {
            get { return References; }
        }

        public bool IsIncluded { get; }

        public TaskDeclarationOrigin Origin { get; }

        public IEnumerable<ISymbol> SymbolsAndSelf() {
            yield return this;

            foreach (var symbol in ConnectionPoints) {
                yield return symbol;
            }
        }
    }
}