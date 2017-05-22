namespace Pharmatechnik.Nav.Language.CodeGen {

    sealed class NodeCodeModelBuilder: SymbolVisitor<NodeCodeModel> {

        public EdgeMode EdgeMode { get; }

        public NodeCodeModelBuilder(EdgeMode edgeMode) {
            EdgeMode = edgeMode;
        }

        public static NodeCodeModel GetNodeCodeModel(INodeSymbol node, IEdgeModeSymbol edgeEdgeMode) {
            var builder = new NodeCodeModelBuilder(edgeEdgeMode.EdgeMode);
            return builder.Visit(node);
        }

        public override NodeCodeModel VisitExitNodeSymbol(IExitNodeSymbol exitNodeSymbol) {
            return new ExitNodeCodeModel(exitNodeSymbol.Name, EdgeMode);
        }

        public override NodeCodeModel VisitEndNodeSymbol(IEndNodeSymbol endNodeSymbol) {
            return new EndNodeCodeModel(endNodeSymbol.Name, EdgeMode);
        }

        public override NodeCodeModel VisitTaskNodeSymbol(ITaskNodeSymbol taskNodeSymbol) {
            return new TaskNodeCodeModel(taskNodeSymbol.Name, EdgeMode);
        }

        public override NodeCodeModel VisitDialogNodeSymbol(IDialogNodeSymbol dialogNodeSymbol) {
            return new GuiNodeCodeModel(dialogNodeSymbol.Name, EdgeMode);
        }

        public override NodeCodeModel VisitViewNodeSymbol(IViewNodeSymbol viewNodeSymbol) {
            return new GuiNodeCodeModel(viewNodeSymbol.Name, EdgeMode);
        }
    }
}