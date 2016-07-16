using System.Collections.Generic;
using System.Linq;

namespace Pharmatechnik.Nav.Language {

    sealed partial class TaskDefinitionSymbol: Symbol, ITaskDefinitionSymbol {

        public TaskDefinitionSymbol(string name, 
                                    Location location, 
                                    TaskDefinitionSyntax syntax, 
                                    TaskDeclarationSymbol taskDeclaration) : base(name, location) {
            Syntax            = syntax;
            AsTaskDeclaration = taskDeclaration;
            NodeDeclarations  = new SymbolCollection<INodeSymbol>();
            Transitions       = new List<Transition>();
            ExitTransitions   = new List<ExitTransition>();
        }

        public TaskDefinitionSyntax Syntax { get; }
        public ITaskDeclarationSymbol AsTaskDeclaration { get; }
        public SymbolCollection<INodeSymbol> NodeDeclarations { get; }
        public List<Transition> Transitions { get; }
        public List<ExitTransition> ExitTransitions { get; }
        
        IReadOnlySymbolCollection<INodeSymbol> ITaskDefinitionSymbol.NodeDeclarations {
            get { return NodeDeclarations; }
        }

        IReadOnlyList<ITransition> ITaskDefinitionSymbol.Transitions {
            get { return Transitions; }
        }

        IReadOnlyList<IExitTransition> ITaskDefinitionSymbol.ExitTransitions {
            get { return ExitTransitions; }
        }

        public IEnumerable<ISymbol> SymbolsAndSelf() {

            yield return this;

            foreach(var symbol in NodeDeclarations.SelectMany(nd => nd.SymbolsAndSelf())) {
                yield return symbol;
            }

            foreach (var symbol in Transitions.SelectMany(t=>t.Symbols())) {
                yield return symbol;
            }

            foreach (var symbol in ExitTransitions.SelectMany(et => et.Symbols())) {
                yield return symbol;
            }
        }
    }    
}