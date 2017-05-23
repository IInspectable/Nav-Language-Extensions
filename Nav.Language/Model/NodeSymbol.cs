#region Using Directives

using System;
using System.Linq;
using System.Collections.Generic;

using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language {
    abstract class NodeSymbol<T> : Symbol, INodeSymbol where T: NodeDeclarationSyntax {

        protected NodeSymbol(string name, Location location, T syntax, TaskDefinitionSymbol containingTask) : base(name, location) {
            Syntax         = syntax         ?? throw new ArgumentNullException(nameof(syntax));
            ContainingTask = containingTask ?? throw new ArgumentNullException(nameof(containingTask));
            References     = new List<INodeReferenceSymbol>();
        }

        [NotNull]
        public T Syntax { get; }

        [NotNull]
        public ITaskDefinitionSymbol ContainingTask { get; }

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
        public abstract IEnumerable<IEdge> GetOutgoingEdges(HashSet<IEdge> seenEdges = null);

        internal static IEnumerable<IEdge> ResolveChoice<TEdge>(TEdge edge) where TEdge : IEdge {
            return ResolveChoice(edge, null);
        }

        internal static IEnumerable<IEdge> ResolveChoice<TEdge>(TEdge edge, HashSet<IEdge> seenEdges) where TEdge : IEdge {

            seenEdges = seenEdges ?? new HashSet<IEdge>();

            if (seenEdges.Contains(edge)) {
                yield break;
            }

            seenEdges.Add(edge);
            yield return edge;

            var choiceNode = edge.Target?.Declaration as IChoiceNodeSymbol;

            if (choiceNode != null) {

                yield return edge;

                foreach (var outgoingEdge in choiceNode.GetOutgoingEdges(seenEdges)) {
                    yield return outgoingEdge;
                }
            }
        }
    }

    abstract class NodeSymbolWithIncomings<TSyntax, TIncomings> : NodeSymbol<TSyntax>
            where TSyntax : NodeDeclarationSyntax
            where TIncomings : IEdge {

        protected NodeSymbolWithIncomings(string name, Location location, TSyntax syntax, TaskDefinitionSymbol containingTask)
            : base(name, location, syntax, containingTask) {
            Incomings = new List<TIncomings>();
        }

        public List<TIncomings> Incomings { get; }

        public override IEnumerable<IEdge> GetIncomingEdges() {
            return Incomings.SelectMany(ResolveChoice);
        }

        public override IEnumerable<IEdge> GetOutgoingEdges(HashSet<IEdge> seenEdges = null) {
            return Enumerable.Empty<IEdge>();
        }
    }

    abstract class NodeSymbolWithOutgoings<TSyntax, TOutgoings> : NodeSymbol<TSyntax>
            where TSyntax : NodeDeclarationSyntax
            where TOutgoings : IEdge {

        protected NodeSymbolWithOutgoings(string name, Location location, TSyntax syntax, TaskDefinitionSymbol containingTask)
            : base(name, location, syntax, containingTask) {
            Outgoings = new List<TOutgoings>();
        }

        public List<TOutgoings> Outgoings { get; }

        public override IEnumerable<IEdge> GetIncomingEdges() {
            return Enumerable.Empty<IEdge>();
        }

        public override IEnumerable<IEdge> GetOutgoingEdges(HashSet<IEdge> seenEdges = null) {
            return Outgoings.SelectMany( e=> ResolveChoice(e, seenEdges));
        }        
    }

    abstract class NodeSymbolWithIncomingsAndOutgoings<TSyntax, TIncomings, TOutgoings> : NodeSymbol<TSyntax>
            where TSyntax    : NodeDeclarationSyntax
            where TIncomings : IEdge
            where TOutgoings : IEdge {

        protected NodeSymbolWithIncomingsAndOutgoings(string name, Location location, TSyntax syntax, TaskDefinitionSymbol containingTask)
            : base(name, location, syntax, containingTask) {
            Incomings = new List<TIncomings>();
            Outgoings = new List<TOutgoings>();
        }

        public List<TIncomings> Incomings { get; }
        public List<TOutgoings> Outgoings { get; }

        public override IEnumerable<IEdge> GetIncomingEdges() {
            return Incomings.SelectMany(ResolveChoice);
        }

        public override IEnumerable<IEdge> GetOutgoingEdges(HashSet<IEdge> seenEdges = null) {
            return Outgoings.SelectMany(e => ResolveChoice(e, seenEdges));
        }
    }

    sealed partial class InitNodeSymbol : NodeSymbolWithOutgoings<InitNodeDeclarationSyntax, ITransition>, IInitNodeSymbol {

        public InitNodeSymbol(string name, Location location, InitNodeDeclarationSyntax syntax, InitNodeAliasSymbol alias, TaskDefinitionSymbol containingTask) 
            : base(name, location, syntax, containingTask) {

            if (alias != null) {
                alias.InitNode = this;
                Alias = alias;
            }
        }

        [CanBeNull]
        public IInitNodeAliasSymbol Alias { get; }

        public override string Name {
            get { return Alias?.Name ?? base.Name; }
        }

        IReadOnlyList<ITransition> IInitNodeSymbol.Outgoings { get { return Outgoings; } }

        public override IEnumerable<ISymbol> SymbolsAndSelf() {
            yield return this;

            if (Alias != null) {
                yield return Alias;
            }
        }
    }

    sealed partial class ExitNodeSymbol : NodeSymbolWithIncomings<ExitNodeDeclarationSyntax, IEdge>, IExitNodeSymbol {

        public ExitNodeSymbol(string name, Location location, ExitNodeDeclarationSyntax syntax, TaskDefinitionSymbol containingTask) 
            : base(name, location, syntax, containingTask) {
        }

        IReadOnlyList<IEdge> IExitNodeSymbol.Incomings { get { return Incomings; } }
    }

    sealed partial class EndNodeSymbol : NodeSymbolWithIncomings<EndNodeDeclarationSyntax, IEdge>, IEndNodeSymbol {

        public EndNodeSymbol(string name, Location location, EndNodeDeclarationSyntax syntax, TaskDefinitionSymbol containingTask) 
            : base(name, location, syntax, containingTask) {
        }

        IReadOnlyList<IEdge> IEndNodeSymbol.Incomings { get { return Incomings; } }
    }

    sealed partial class TaskNodeSymbol : NodeSymbolWithIncomingsAndOutgoings<TaskNodeDeclarationSyntax, IEdge, IExitTransition>, ITaskNodeSymbol {

        public TaskNodeSymbol(string name, Location location, TaskNodeDeclarationSyntax syntax, TaskNodeAliasSymbol alias, 
                             TaskDeclarationSymbol declaration, TaskDefinitionSymbol containingTask) 
            : base(name, location, syntax, containingTask) {
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

        public DialogNodeSymbol(string name, Location location, DialogNodeDeclarationSyntax syntax, TaskDefinitionSymbol containingTask) 
            : base(name, location, syntax, containingTask) {
        }

        IReadOnlyList<IEdge> IGuiNodeSymbol.Incomings { get { return Incomings; } }
        IReadOnlyList<ITransition> IGuiNodeSymbol.Outgoings { get { return Outgoings; } }
    }

    sealed partial class ViewNodeSymbol : NodeSymbolWithIncomingsAndOutgoings<ViewNodeDeclarationSyntax, IEdge, ITransition>, IViewNodeSymbol {

        public ViewNodeSymbol(string name, Location location, ViewNodeDeclarationSyntax syntax, TaskDefinitionSymbol containingTask) 
            : base(name, location, syntax, containingTask) {          
        }

        IReadOnlyList<IEdge> IGuiNodeSymbol.Incomings { get { return Incomings; } }
        IReadOnlyList<ITransition> IGuiNodeSymbol.Outgoings { get { return Outgoings; } }
    }

    sealed partial class ChoiceNodeSymbol : NodeSymbolWithIncomingsAndOutgoings<ChoiceNodeDeclarationSyntax, IEdge, ITransition>, IChoiceNodeSymbol {

        public ChoiceNodeSymbol(string name, Location location, ChoiceNodeDeclarationSyntax syntax, TaskDefinitionSymbol containingTask) 
            : base(name, location, syntax, containingTask) {
        }

        IReadOnlyList<IEdge> IChoiceNodeSymbol.Incomings { get { return Incomings; } }
        IReadOnlyList<ITransition> IChoiceNodeSymbol.Outgoings { get { return Outgoings; } }
    }
}