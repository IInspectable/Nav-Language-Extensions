﻿using System.Collections.Generic;

using JetBrains.Annotations;

namespace Pharmatechnik.Nav.Language; 

sealed class TriggerTransition: Transition, ITriggerTransition {

    public TriggerTransition(TransitionDefinitionSyntax syntax,
                             ITaskDefinitionSymbol containingTask,
                             GuiNodeReferenceSymbol sourceReference,
                             EdgeModeSymbol edgeMode,
                             NodeReferenceSymbol targetReference,
                             ConcatTransition concatTransition,
                             SymbolCollection<TriggerSymbol> triggers)
        : base(syntax, containingTask, sourceReference, edgeMode, targetReference, concatTransition) {

        Triggers = triggers ?? new SymbolCollection<TriggerSymbol>();

        foreach (var trigger in Triggers) {
            trigger.Transition = this;
        }
    }

    public IGuiNodeReferenceSymbol GuiNodeSourceReference => (IGuiNodeReferenceSymbol) SourceReference;

    IReadOnlySymbolCollection<ITriggerSymbol> ITriggerTransition.Triggers => Triggers;

    [NotNull]
    public SymbolCollection<TriggerSymbol> Triggers { get; }

    [NotNull]
    public override IEnumerable<ISymbol> Symbols() {

        foreach (var symbol in base.Symbols()) {
            yield return symbol;
        }

        foreach (var trigger in Triggers) {
            yield return trigger;
        }
    }

}