using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Pharmatechnik.Nav.Language {

    abstract class NodeSymbol<T> : Symbol, INodeSymbol where T: NodeDeclarationSyntax {

        protected NodeSymbol(string name, Location location, T syntax) : base(name, location) {

            if (syntax == null) {
                throw new ArgumentNullException(nameof(syntax));
            }

            Syntax = syntax;
            References = new List<INodeReferenceSymbol>();
        }

        [NotNull]
        public T Syntax { get; }

        public List<INodeReferenceSymbol> References { get; }

        IReadOnlyList<INodeReferenceSymbol> INodeSymbol.References {
            get { return References; }
        }

        NodeDeclarationSyntax INodeSymbol.Syntax {
            get { return Syntax; }
        }

        public virtual IEnumerable<ISymbol> SymbolsAndSelf() {
            yield return this;
        }

        public abstract IEnumerable<IEdge> GetIncomingEdges();
        public abstract IEnumerable<IEdge> GetOutgoingEdges();

        internal static IEnumerable<IEdge> ResolveChoice<TEdge>(TEdge edge) where TEdge : IEdge {

            var choiceNode = edge.Target?.Declaration as IChoiceNodeSymbol;
            if (choiceNode != null) {

                yield return edge;

                foreach (var node in choiceNode.GetOutgoingEdges()) {
                    yield return node;
                }
            } else {
                yield return edge;
            }
        }
    }

    abstract class NodeSymbolWithIncomings<TSyntax, TIncomings> : NodeSymbol<TSyntax>
            where TSyntax : NodeDeclarationSyntax
            where TIncomings : IEdge {

        protected NodeSymbolWithIncomings(string name, Location location, TSyntax syntax) : base(name, location, syntax) {
            Incomings = new List<TIncomings>();
        }

        public List<TIncomings> Incomings { get; }

        public override IEnumerable<IEdge> GetIncomingEdges() {
            return Incomings.SelectMany(ResolveChoice);
        }

        public override IEnumerable<IEdge> GetOutgoingEdges() {
            return Enumerable.Empty<IEdge>();
        }
    }

    abstract class NodeSymbolWithOutgoings<TSyntax, TOutgoings> : NodeSymbol<TSyntax>
            where TSyntax : NodeDeclarationSyntax
            where TOutgoings : IEdge {

        protected NodeSymbolWithOutgoings(string name, Location location, TSyntax syntax) : base(name, location, syntax) {
            Outgoings = new List<TOutgoings>();
        }

        public List<TOutgoings> Outgoings { get; }

        public override IEnumerable<IEdge> GetIncomingEdges() {
            return Enumerable.Empty<IEdge>();
        }

        public override IEnumerable<IEdge> GetOutgoingEdges() {
            return Outgoings.SelectMany(ResolveChoice);
        }        
    }

    abstract class NodeSymbolWithIncomingsAndOutgoings<TSyntax, TIncomings, TOutgoings> : NodeSymbol<TSyntax>
            where TSyntax    : NodeDeclarationSyntax
            where TIncomings : IEdge
            where TOutgoings : IEdge {

        protected NodeSymbolWithIncomingsAndOutgoings(string name, Location location, TSyntax syntax) : base(name, location, syntax) {
            Incomings = new List<TIncomings>();
            Outgoings = new List<TOutgoings>();
        }

        public List<TIncomings> Incomings { get; }
        public List<TOutgoings> Outgoings { get; }

        public override IEnumerable<IEdge> GetIncomingEdges() {
            return Incomings.SelectMany(ResolveChoice);
        }

        public override IEnumerable<IEdge> GetOutgoingEdges() {
            return Outgoings.SelectMany(ResolveChoice);
        }
    }

    sealed partial class InitNodeSymbol : NodeSymbolWithOutgoings<InitNodeDeclarationSyntax, ITransition>, IInitNodeSymbol {

        public InitNodeSymbol(string name, Location location, InitNodeDeclarationSyntax syntax) : base(name, location, syntax) {
        }

        IReadOnlyList<ITransition> IInitNodeSymbol.Outgoings { get { return Outgoings; } }
    }

    sealed partial class ExitNodeSymbol : NodeSymbolWithIncomings<ExitNodeDeclarationSyntax, IEdge>, IExitNodeSymbol {

        public ExitNodeSymbol(string name, Location location, ExitNodeDeclarationSyntax syntax) : base(name, location, syntax) {
        }

        IReadOnlyList<IEdge> IExitNodeSymbol.Incomings { get { return Incomings; } }
    }

    sealed partial class EndNodeSymbol : NodeSymbolWithIncomings<EndNodeDeclarationSyntax, IEdge>, IEndNodeSymbol {

        public EndNodeSymbol(string name, Location location, EndNodeDeclarationSyntax syntax) : base(name, location, syntax) {
        }

        IReadOnlyList<IEdge> IEndNodeSymbol.Incomings { get { return Incomings; } }
    }

    sealed partial class TaskNodeSymbol : NodeSymbolWithIncomingsAndOutgoings<TaskNodeDeclarationSyntax, IEdge, IExitTransition>, ITaskNodeSymbol {

        public TaskNodeSymbol(string name, Location location, TaskNodeDeclarationSyntax syntax, TaskNodeAliasSymbol alias, TaskDeclarationSymbol declaration) : base(name, location, syntax) {
            Declaration = declaration;
            
            if(alias != null) {
                alias.TaskNode = this;
                Alias          = alias;
            }
        }

        public override string Name {
            get { return Alias?.Name ?? base.Name; }
        }

        [CanBeNull]
        public TaskDeclarationSymbol Declaration { get; }

        [CanBeNull]
        ITaskDeclarationSymbol ITaskNodeSymbol.Declaration {
            get { return Declaration; }
        }

        [CanBeNull]
        public ITaskNodeAliasSymbol Alias { get; }

        IReadOnlyList<IEdge> ITaskNodeSymbol.Incomings { get { return Incomings; } }
        IReadOnlyList<IExitTransition> ITaskNodeSymbol.Outgoings { get { return Outgoings; } }

        public override IEnumerable<ISymbol> SymbolsAndSelf() {
            yield return this;

            if(Alias != null) {
                yield return Alias;
            }
        }

    }

    sealed partial class DialogNodeSymbol : NodeSymbolWithIncomingsAndOutgoings<DialogNodeDeclarationSyntax, IEdge, ITransition>, IDialogNodeSymbol {

        public DialogNodeSymbol(string name, Location location, DialogNodeDeclarationSyntax syntax) : base(name, location, syntax) {
        }

        IReadOnlyList<IEdge> IDialogNodeSymbol.Incomings { get { return Incomings; } }
        IReadOnlyList<ITransition> IDialogNodeSymbol.Outgoings { get { return Outgoings; } }
    }

    sealed partial class ViewNodeSymbol : NodeSymbolWithIncomingsAndOutgoings<ViewNodeDeclarationSyntax, IEdge, ITransition>, IViewNodeSymbol {

        public ViewNodeSymbol(string name, Location location, ViewNodeDeclarationSyntax syntax) : base(name, location, syntax) {          
        }

        IReadOnlyList<IEdge> IViewNodeSymbol.Incomings { get { return Incomings; } }
        IReadOnlyList<ITransition> IViewNodeSymbol.Outgoings { get { return Outgoings; } }
    }

    sealed partial class ChoiceNodeSymbol : NodeSymbolWithIncomingsAndOutgoings<ChoiceNodeDeclarationSyntax, IEdge, ITransition>, IChoiceNodeSymbol {

        public ChoiceNodeSymbol(string name, Location location, ChoiceNodeDeclarationSyntax syntax) : base(name, location, syntax) {
        }

        IReadOnlyList<IEdge> IChoiceNodeSymbol.Incomings { get { return Incomings; } }
        IReadOnlyList<ITransition> IChoiceNodeSymbol.Outgoings { get { return Outgoings; } }
    }
}