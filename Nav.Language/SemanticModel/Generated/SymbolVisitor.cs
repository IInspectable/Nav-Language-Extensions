namespace Pharmatechnik.Nav.Language {

    partial interface ISymbol {

        void Accept(ISymbolVisitor visitor);
        T Accept<T>(ISymbolVisitor<T> visitor);

    }

    public interface ISymbolVisitor {

        void VisitConnectionPointReferenceSymbol(IConnectionPointReferenceSymbol connectionPointReferenceSymbol);
        void VisitInitConnectionPointSymbol(IInitConnectionPointSymbol initConnectionPointSymbol);
        void VisitExitConnectionPointSymbol(IExitConnectionPointSymbol exitConnectionPointSymbol);
        void VisitEndConnectionPointSymbol(IEndConnectionPointSymbol endConnectionPointSymbol);
        void VisitEdgeModeSymbol(IEdgeModeSymbol edgeModeSymbol);
        void VisitNodeReferenceSymbol(INodeReferenceSymbol nodeReferenceSymbol);
        void VisitInitNodeSymbol(IInitNodeSymbol initNodeSymbol);
        void VisitExitNodeSymbol(IExitNodeSymbol exitNodeSymbol);
        void VisitEndNodeSymbol(IEndNodeSymbol endNodeSymbol);
        void VisitTaskNodeSymbol(ITaskNodeSymbol taskNodeSymbol);
        void VisitDialogNodeSymbol(IDialogNodeSymbol dialogNodeSymbol);
        void VisitViewNodeSymbol(IViewNodeSymbol viewNodeSymbol);
        void VisitChoiceNodeSymbol(IChoiceNodeSymbol choiceNodeSymbol);
        void VisitTaskDeclarationSymbol(ITaskDeclarationSymbol taskDeclarationSymbol);
        void VisitTaskDefinitionSymbol(ITaskDefinitionSymbol taskDefinitionSymbol);
        void VisitSignalTriggerSymbol(ISignalTriggerSymbol signalTriggerSymbol);
        void VisitSpontaneousTriggerSymbol(ISpontaneousTriggerSymbol spontaneousTriggerSymbol);
        void VisitTaskNodeAliasSymbol(ITaskNodeAliasSymbol taskNodeAliasSymbol);
        void VisitIncludeSymbol(IIncludeSymbol includeSymbol);
        void VisitInitNodeAliasSymbol(IInitNodeAliasSymbol initNodeAliasSymbol);
        void VisitInitNodeReferenceSymbol(IInitNodeReferenceSymbol initNodeReferenceSymbol);
        void VisitChoiceNodeReferenceSymbol(IChoiceNodeReferenceSymbol choiceNodeReferenceSymbol);
        void VisitGuiNodeReferenceSymbol(IGuiNodeReferenceSymbol guiNodeReferenceSymbol);
        void VisitTaskNodeReferenceSymbol(ITaskNodeReferenceSymbol taskNodeReferenceSymbol);

    }

    public interface ISymbolVisitor<T> {

        T VisitConnectionPointReferenceSymbol(IConnectionPointReferenceSymbol connectionPointReferenceSymbol);
        T VisitInitConnectionPointSymbol(IInitConnectionPointSymbol initConnectionPointSymbol);
        T VisitExitConnectionPointSymbol(IExitConnectionPointSymbol exitConnectionPointSymbol);
        T VisitEndConnectionPointSymbol(IEndConnectionPointSymbol endConnectionPointSymbol);
        T VisitEdgeModeSymbol(IEdgeModeSymbol edgeModeSymbol);
        T VisitNodeReferenceSymbol(INodeReferenceSymbol nodeReferenceSymbol);
        T VisitInitNodeSymbol(IInitNodeSymbol initNodeSymbol);
        T VisitExitNodeSymbol(IExitNodeSymbol exitNodeSymbol);
        T VisitEndNodeSymbol(IEndNodeSymbol endNodeSymbol);
        T VisitTaskNodeSymbol(ITaskNodeSymbol taskNodeSymbol);
        T VisitDialogNodeSymbol(IDialogNodeSymbol dialogNodeSymbol);
        T VisitViewNodeSymbol(IViewNodeSymbol viewNodeSymbol);
        T VisitChoiceNodeSymbol(IChoiceNodeSymbol choiceNodeSymbol);
        T VisitTaskDeclarationSymbol(ITaskDeclarationSymbol taskDeclarationSymbol);
        T VisitTaskDefinitionSymbol(ITaskDefinitionSymbol taskDefinitionSymbol);
        T VisitSignalTriggerSymbol(ISignalTriggerSymbol signalTriggerSymbol);
        T VisitSpontaneousTriggerSymbol(ISpontaneousTriggerSymbol spontaneousTriggerSymbol);
        T VisitTaskNodeAliasSymbol(ITaskNodeAliasSymbol taskNodeAliasSymbol);
        T VisitIncludeSymbol(IIncludeSymbol includeSymbol);
        T VisitInitNodeAliasSymbol(IInitNodeAliasSymbol initNodeAliasSymbol);
        T VisitInitNodeReferenceSymbol(IInitNodeReferenceSymbol initNodeReferenceSymbol);
        T VisitChoiceNodeReferenceSymbol(IChoiceNodeReferenceSymbol choiceNodeReferenceSymbol);
        T VisitGuiNodeReferenceSymbol(IGuiNodeReferenceSymbol guiNodeReferenceSymbol);
        T VisitTaskNodeReferenceSymbol(ITaskNodeReferenceSymbol taskNodeReferenceSymbol);

    }

    partial class Symbol {

        public abstract void Accept(ISymbolVisitor visitor);
        public abstract T Accept<T>(ISymbolVisitor<T> visitor);

    }

    partial class ConnectionPointReferenceSymbol {

        public override void Accept(ISymbolVisitor visitor) {
            visitor.VisitConnectionPointReferenceSymbol(this);
        }

        public override T Accept<T>(ISymbolVisitor<T> visitor) {
            return visitor.VisitConnectionPointReferenceSymbol(this);
        }

    }

    partial class InitConnectionPointSymbol {

        public override void Accept(ISymbolVisitor visitor) {
            visitor.VisitInitConnectionPointSymbol(this);
        }

        public override T Accept<T>(ISymbolVisitor<T> visitor) {
            return visitor.VisitInitConnectionPointSymbol(this);
        }

    }

    partial class ExitConnectionPointSymbol {

        public override void Accept(ISymbolVisitor visitor) {
            visitor.VisitExitConnectionPointSymbol(this);
        }

        public override T Accept<T>(ISymbolVisitor<T> visitor) {
            return visitor.VisitExitConnectionPointSymbol(this);
        }

    }

    partial class EndConnectionPointSymbol {

        public override void Accept(ISymbolVisitor visitor) {
            visitor.VisitEndConnectionPointSymbol(this);
        }

        public override T Accept<T>(ISymbolVisitor<T> visitor) {
            return visitor.VisitEndConnectionPointSymbol(this);
        }

    }

    partial class EdgeModeSymbol {

        public override void Accept(ISymbolVisitor visitor) {
            visitor.VisitEdgeModeSymbol(this);
        }

        public override T Accept<T>(ISymbolVisitor<T> visitor) {
            return visitor.VisitEdgeModeSymbol(this);
        }

    }

    partial class NodeReferenceSymbol {

        public override void Accept(ISymbolVisitor visitor) {
            visitor.VisitNodeReferenceSymbol(this);
        }

        public override T Accept<T>(ISymbolVisitor<T> visitor) {
            return visitor.VisitNodeReferenceSymbol(this);
        }

    }

    partial class InitNodeSymbol {

        public override void Accept(ISymbolVisitor visitor) {
            visitor.VisitInitNodeSymbol(this);
        }

        public override T Accept<T>(ISymbolVisitor<T> visitor) {
            return visitor.VisitInitNodeSymbol(this);
        }

    }

    partial class ExitNodeSymbol {

        public override void Accept(ISymbolVisitor visitor) {
            visitor.VisitExitNodeSymbol(this);
        }

        public override T Accept<T>(ISymbolVisitor<T> visitor) {
            return visitor.VisitExitNodeSymbol(this);
        }

    }

    partial class EndNodeSymbol {

        public override void Accept(ISymbolVisitor visitor) {
            visitor.VisitEndNodeSymbol(this);
        }

        public override T Accept<T>(ISymbolVisitor<T> visitor) {
            return visitor.VisitEndNodeSymbol(this);
        }

    }

    partial class TaskNodeSymbol {

        public override void Accept(ISymbolVisitor visitor) {
            visitor.VisitTaskNodeSymbol(this);
        }

        public override T Accept<T>(ISymbolVisitor<T> visitor) {
            return visitor.VisitTaskNodeSymbol(this);
        }

    }

    partial class DialogNodeSymbol {

        public override void Accept(ISymbolVisitor visitor) {
            visitor.VisitDialogNodeSymbol(this);
        }

        public override T Accept<T>(ISymbolVisitor<T> visitor) {
            return visitor.VisitDialogNodeSymbol(this);
        }

    }

    partial class ViewNodeSymbol {

        public override void Accept(ISymbolVisitor visitor) {
            visitor.VisitViewNodeSymbol(this);
        }

        public override T Accept<T>(ISymbolVisitor<T> visitor) {
            return visitor.VisitViewNodeSymbol(this);
        }

    }

    partial class ChoiceNodeSymbol {

        public override void Accept(ISymbolVisitor visitor) {
            visitor.VisitChoiceNodeSymbol(this);
        }

        public override T Accept<T>(ISymbolVisitor<T> visitor) {
            return visitor.VisitChoiceNodeSymbol(this);
        }

    }

    partial class TaskDeclarationSymbol {

        public override void Accept(ISymbolVisitor visitor) {
            visitor.VisitTaskDeclarationSymbol(this);
        }

        public override T Accept<T>(ISymbolVisitor<T> visitor) {
            return visitor.VisitTaskDeclarationSymbol(this);
        }

    }

    partial class TaskDefinitionSymbol {

        public override void Accept(ISymbolVisitor visitor) {
            visitor.VisitTaskDefinitionSymbol(this);
        }

        public override T Accept<T>(ISymbolVisitor<T> visitor) {
            return visitor.VisitTaskDefinitionSymbol(this);
        }

    }

    partial class SignalTriggerSymbol {

        public override void Accept(ISymbolVisitor visitor) {
            visitor.VisitSignalTriggerSymbol(this);
        }

        public override T Accept<T>(ISymbolVisitor<T> visitor) {
            return visitor.VisitSignalTriggerSymbol(this);
        }

    }

    partial class SpontaneousTriggerSymbol {

        public override void Accept(ISymbolVisitor visitor) {
            visitor.VisitSpontaneousTriggerSymbol(this);
        }

        public override T Accept<T>(ISymbolVisitor<T> visitor) {
            return visitor.VisitSpontaneousTriggerSymbol(this);
        }

    }

    partial class TaskNodeAliasSymbol {

        public override void Accept(ISymbolVisitor visitor) {
            visitor.VisitTaskNodeAliasSymbol(this);
        }

        public override T Accept<T>(ISymbolVisitor<T> visitor) {
            return visitor.VisitTaskNodeAliasSymbol(this);
        }

    }

    partial class IncludeSymbol {

        public override void Accept(ISymbolVisitor visitor) {
            visitor.VisitIncludeSymbol(this);
        }

        public override T Accept<T>(ISymbolVisitor<T> visitor) {
            return visitor.VisitIncludeSymbol(this);
        }

    }

    partial class InitNodeAliasSymbol {

        public override void Accept(ISymbolVisitor visitor) {
            visitor.VisitInitNodeAliasSymbol(this);
        }

        public override T Accept<T>(ISymbolVisitor<T> visitor) {
            return visitor.VisitInitNodeAliasSymbol(this);
        }

    }

    partial class InitNodeReferenceSymbol {

        public override void Accept(ISymbolVisitor visitor) {
            visitor.VisitInitNodeReferenceSymbol(this);
        }

        public override T Accept<T>(ISymbolVisitor<T> visitor) {
            return visitor.VisitInitNodeReferenceSymbol(this);
        }

    }

    partial class ChoiceNodeReferenceSymbol {

        public override void Accept(ISymbolVisitor visitor) {
            visitor.VisitChoiceNodeReferenceSymbol(this);
        }

        public override T Accept<T>(ISymbolVisitor<T> visitor) {
            return visitor.VisitChoiceNodeReferenceSymbol(this);
        }

    }

    partial class GuiNodeReferenceSymbol {

        public override void Accept(ISymbolVisitor visitor) {
            visitor.VisitGuiNodeReferenceSymbol(this);
        }

        public override T Accept<T>(ISymbolVisitor<T> visitor) {
            return visitor.VisitGuiNodeReferenceSymbol(this);
        }

    }

    partial class TaskNodeReferenceSymbol {

        public override void Accept(ISymbolVisitor visitor) {
            visitor.VisitTaskNodeReferenceSymbol(this);
        }

        public override T Accept<T>(ISymbolVisitor<T> visitor) {
            return visitor.VisitTaskNodeReferenceSymbol(this);
        }

    }

    public abstract class SymbolVisitor: ISymbolVisitor {

        public void Visit(ISymbol symbol) {
            symbol.Accept(this);
        }

        protected virtual void DefaultVisit(ISymbol symbol) {
        }

        public virtual void VisitConnectionPointReferenceSymbol(IConnectionPointReferenceSymbol connectionPointReferenceSymbol) {
            DefaultVisit(connectionPointReferenceSymbol);
        }

        public virtual void VisitInitConnectionPointSymbol(IInitConnectionPointSymbol initConnectionPointSymbol) {
            DefaultVisit(initConnectionPointSymbol);
        }

        public virtual void VisitExitConnectionPointSymbol(IExitConnectionPointSymbol exitConnectionPointSymbol) {
            DefaultVisit(exitConnectionPointSymbol);
        }

        public virtual void VisitEndConnectionPointSymbol(IEndConnectionPointSymbol endConnectionPointSymbol) {
            DefaultVisit(endConnectionPointSymbol);
        }

        public virtual void VisitEdgeModeSymbol(IEdgeModeSymbol edgeModeSymbol) {
            DefaultVisit(edgeModeSymbol);
        }

        public virtual void VisitNodeReferenceSymbol(INodeReferenceSymbol nodeReferenceSymbol) {
            DefaultVisit(nodeReferenceSymbol);
        }

        public virtual void VisitInitNodeSymbol(IInitNodeSymbol initNodeSymbol) {
            DefaultVisit(initNodeSymbol);
        }

        public virtual void VisitExitNodeSymbol(IExitNodeSymbol exitNodeSymbol) {
            DefaultVisit(exitNodeSymbol);
        }

        public virtual void VisitEndNodeSymbol(IEndNodeSymbol endNodeSymbol) {
            DefaultVisit(endNodeSymbol);
        }

        public virtual void VisitTaskNodeSymbol(ITaskNodeSymbol taskNodeSymbol) {
            DefaultVisit(taskNodeSymbol);
        }

        public virtual void VisitDialogNodeSymbol(IDialogNodeSymbol dialogNodeSymbol) {
            DefaultVisit(dialogNodeSymbol);
        }

        public virtual void VisitViewNodeSymbol(IViewNodeSymbol viewNodeSymbol) {
            DefaultVisit(viewNodeSymbol);
        }

        public virtual void VisitChoiceNodeSymbol(IChoiceNodeSymbol choiceNodeSymbol) {
            DefaultVisit(choiceNodeSymbol);
        }

        public virtual void VisitTaskDeclarationSymbol(ITaskDeclarationSymbol taskDeclarationSymbol) {
            DefaultVisit(taskDeclarationSymbol);
        }

        public virtual void VisitTaskDefinitionSymbol(ITaskDefinitionSymbol taskDefinitionSymbol) {
            DefaultVisit(taskDefinitionSymbol);
        }

        public virtual void VisitSignalTriggerSymbol(ISignalTriggerSymbol signalTriggerSymbol) {
            DefaultVisit(signalTriggerSymbol);
        }

        public virtual void VisitSpontaneousTriggerSymbol(ISpontaneousTriggerSymbol spontaneousTriggerSymbol) {
            DefaultVisit(spontaneousTriggerSymbol);
        }

        public virtual void VisitTaskNodeAliasSymbol(ITaskNodeAliasSymbol taskNodeAliasSymbol) {
            DefaultVisit(taskNodeAliasSymbol);
        }

        public virtual void VisitIncludeSymbol(IIncludeSymbol includeSymbol) {
            DefaultVisit(includeSymbol);
        }

        public virtual void VisitInitNodeAliasSymbol(IInitNodeAliasSymbol initNodeAliasSymbol) {
            DefaultVisit(initNodeAliasSymbol);
        }

        public virtual void VisitInitNodeReferenceSymbol(IInitNodeReferenceSymbol initNodeReferenceSymbol) {
            VisitNodeReferenceSymbol(initNodeReferenceSymbol);
        }

        public virtual void VisitChoiceNodeReferenceSymbol(IChoiceNodeReferenceSymbol choiceNodeReferenceSymbol) {
            VisitNodeReferenceSymbol(choiceNodeReferenceSymbol);
        }

        public virtual void VisitGuiNodeReferenceSymbol(IGuiNodeReferenceSymbol guiNodeReferenceSymbol) {
            VisitNodeReferenceSymbol(guiNodeReferenceSymbol);
        }

        public virtual void VisitTaskNodeReferenceSymbol(ITaskNodeReferenceSymbol taskNodeReferenceSymbol) {
            VisitNodeReferenceSymbol(taskNodeReferenceSymbol);
        }

    }

    public abstract class SymbolVisitor<T>: ISymbolVisitor<T> {

        public T Visit(ISymbol symbol) {
            return symbol.Accept(this);
        }

        protected virtual T DefaultVisit(ISymbol symbol) {
            return default(T);
        }

        public virtual T VisitConnectionPointReferenceSymbol(IConnectionPointReferenceSymbol connectionPointReferenceSymbol) {
            return DefaultVisit(connectionPointReferenceSymbol);
        }

        public virtual T VisitInitConnectionPointSymbol(IInitConnectionPointSymbol initConnectionPointSymbol) {
            return DefaultVisit(initConnectionPointSymbol);
        }

        public virtual T VisitExitConnectionPointSymbol(IExitConnectionPointSymbol exitConnectionPointSymbol) {
            return DefaultVisit(exitConnectionPointSymbol);
        }

        public virtual T VisitEndConnectionPointSymbol(IEndConnectionPointSymbol endConnectionPointSymbol) {
            return DefaultVisit(endConnectionPointSymbol);
        }

        public virtual T VisitEdgeModeSymbol(IEdgeModeSymbol edgeModeSymbol) {
            return DefaultVisit(edgeModeSymbol);
        }

        public virtual T VisitNodeReferenceSymbol(INodeReferenceSymbol nodeReferenceSymbol) {
            return DefaultVisit(nodeReferenceSymbol);
        }

        public virtual T VisitInitNodeSymbol(IInitNodeSymbol initNodeSymbol) {
            return DefaultVisit(initNodeSymbol);
        }

        public virtual T VisitExitNodeSymbol(IExitNodeSymbol exitNodeSymbol) {
            return DefaultVisit(exitNodeSymbol);
        }

        public virtual T VisitEndNodeSymbol(IEndNodeSymbol endNodeSymbol) {
            return DefaultVisit(endNodeSymbol);
        }

        public virtual T VisitTaskNodeSymbol(ITaskNodeSymbol taskNodeSymbol) {
            return DefaultVisit(taskNodeSymbol);
        }

        public virtual T VisitDialogNodeSymbol(IDialogNodeSymbol dialogNodeSymbol) {
            return DefaultVisit(dialogNodeSymbol);
        }

        public virtual T VisitViewNodeSymbol(IViewNodeSymbol viewNodeSymbol) {
            return DefaultVisit(viewNodeSymbol);
        }

        public virtual T VisitChoiceNodeSymbol(IChoiceNodeSymbol choiceNodeSymbol) {
            return DefaultVisit(choiceNodeSymbol);
        }

        public virtual T VisitTaskDeclarationSymbol(ITaskDeclarationSymbol taskDeclarationSymbol) {
            return DefaultVisit(taskDeclarationSymbol);
        }

        public virtual T VisitTaskDefinitionSymbol(ITaskDefinitionSymbol taskDefinitionSymbol) {
            return DefaultVisit(taskDefinitionSymbol);
        }

        public virtual T VisitSignalTriggerSymbol(ISignalTriggerSymbol signalTriggerSymbol) {
            return DefaultVisit(signalTriggerSymbol);
        }

        public virtual T VisitSpontaneousTriggerSymbol(ISpontaneousTriggerSymbol spontaneousTriggerSymbol) {
            return DefaultVisit(spontaneousTriggerSymbol);
        }

        public virtual T VisitTaskNodeAliasSymbol(ITaskNodeAliasSymbol taskNodeAliasSymbol) {
            return DefaultVisit(taskNodeAliasSymbol);
        }

        public virtual T VisitIncludeSymbol(IIncludeSymbol includeSymbol) {
            return DefaultVisit(includeSymbol);
        }

        public virtual T VisitInitNodeAliasSymbol(IInitNodeAliasSymbol initNodeAliasSymbol) {
            return DefaultVisit(initNodeAliasSymbol);
        }

        public virtual T VisitInitNodeReferenceSymbol(IInitNodeReferenceSymbol initNodeReferenceSymbol) {
            return VisitNodeReferenceSymbol(initNodeReferenceSymbol);
        }

        public virtual T VisitChoiceNodeReferenceSymbol(IChoiceNodeReferenceSymbol choiceNodeReferenceSymbol) {
            return VisitNodeReferenceSymbol(choiceNodeReferenceSymbol);
        }

        public virtual T VisitGuiNodeReferenceSymbol(IGuiNodeReferenceSymbol guiNodeReferenceSymbol) {
            return VisitNodeReferenceSymbol(guiNodeReferenceSymbol);
        }

        public virtual T VisitTaskNodeReferenceSymbol(ITaskNodeReferenceSymbol taskNodeReferenceSymbol) {
            return VisitNodeReferenceSymbol(taskNodeReferenceSymbol);
        }

    }

}