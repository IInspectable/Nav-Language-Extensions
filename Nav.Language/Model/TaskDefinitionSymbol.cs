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
            InitTransitions   = new List<InitTransition>();
            TriggerTransitions= new List<TriggerTransition>();
            ExitTransitions   = new List<ExitTransition>();
        }

        public TaskDefinitionSyntax Syntax { get; }
        public ITaskDeclarationSymbol AsTaskDeclaration { get; }
        public SymbolCollection<INodeSymbol> NodeDeclarations { get; }
        public List<Transition> Transitions { get; }

        public List<InitTransition> InitTransitions { get; }
        public List<TriggerTransition> TriggerTransitions { get; }

        public List<ExitTransition> ExitTransitions { get; }
        public CodeGenerationUnit CodeGenerationUnit { get; private set; }

        IReadOnlySymbolCollection<INodeSymbol> ITaskDefinitionSymbol.NodeDeclarations => NodeDeclarations;
        IReadOnlyList<IInitTransition> ITaskDefinitionSymbol.InitTransitions          => InitTransitions;
        IReadOnlyList<ITriggerTransition> ITaskDefinitionSymbol.TriggerTransitions    => TriggerTransitions;
        IReadOnlyList<IExitTransition> ITaskDefinitionSymbol.ExitTransitions          => ExitTransitions;

        public IEnumerable<ISymbol> SymbolsAndSelf() {

            yield return this;

            foreach(var symbol in NodeDeclarations.SelectMany(nd => nd.SymbolsAndSelf())) {
                yield return symbol;
            }

            foreach (var symbol in InitTransitions.SelectMany(t=>t.Symbols())) {
                yield return symbol;
            }

            foreach (var symbol in TriggerTransitions.SelectMany(t=>t.Symbols())) {
                yield return symbol;
            }

            foreach (var symbol in ExitTransitions.SelectMany(et => et.Symbols())) {
                yield return symbol;
            }
        }

        internal void FinalConstruct(CodeGenerationUnit codeGenerationUnit) {
            CodeGenerationUnit = codeGenerationUnit;
        }
    }    
}