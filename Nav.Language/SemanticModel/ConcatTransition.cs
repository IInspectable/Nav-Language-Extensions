using System;
using System.Collections.Generic;

using JetBrains.Annotations;

namespace Pharmatechnik.Nav.Language;

sealed class ConcatTransition: IConcatTransition {

    internal ConcatTransition(ConcatTransitionSyntax syntax,
                              TaskDefinitionSymbol containingTask,
                              GuiNodeReferenceSymbol sourceReference,
                              EdgeModeSymbol edgeMode,
                              TaskNodeReferenceSymbol targetReference) {

        ContainingTask  = containingTask ?? throw new ArgumentNullException(nameof(containingTask));
        Syntax          = syntax         ?? throw new ArgumentNullException(nameof(syntax));
        SourceReference = sourceReference;
        EdgeMode        = edgeMode;
        TargetReference = targetReference;

        if (sourceReference != null) {
            sourceReference.Edge = this;
        }

        if (edgeMode != null) {
            edgeMode.Edge = this;
        }

        if (targetReference != null) {
            targetReference.Edge = this;
        }
    }

    public ConcatTransitionSyntax Syntax { get; }

    public ITaskDefinitionSymbol ContainingTask { get; }

    [NotNull]
    public Location Location => Syntax.GetLocation();

    INodeReferenceSymbol IEdge.SourceReference => SourceReference;

    public IGuiNodeReferenceSymbol SourceReference { get; }

    public IEdgeModeSymbol EdgeMode { get; }

    INodeReferenceSymbol IEdge.     TargetReference => TargetReference;
    public ITaskNodeReferenceSymbol TargetReference { get; }

    public IEnumerable<ISymbol> Symbols() {

        if (SourceReference != null) {
            yield return SourceReference;
        }

        if (EdgeMode != null) {
            yield return EdgeMode;
        }

        if (TargetReference != null) {
            yield return TargetReference;
        }
    }

}