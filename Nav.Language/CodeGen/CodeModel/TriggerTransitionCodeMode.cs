#nullable enable

#region Using Directives

using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen; 

class TriggerTransitionCodeModel: TransitionCodeModel {

    readonly SignalTriggerCodeInfo _triggerCodeInfo;

    public TriggerTransitionCodeModel(TaskCodeInfo containingTask,
                                      SignalTriggerCodeInfo triggerCodeInfo,
                                      ImmutableList<Call> reachableCalls)
        : base(containingTask, reachableCalls) {
        _triggerCodeInfo = triggerCodeInfo ?? throw new ArgumentNullException(nameof(triggerCodeInfo));
        ViewParameter    = new ParameterCodeModel(triggerCodeInfo.TOClassName, CodeGenFacts.ToParamtername);
    }

    public string TriggerName => _triggerCodeInfo.TriggerName;

    public override string GetCallContextClassName() => $"{TriggerName}{CodeGenFacts.CallContextClassSuffix}";

    public ParameterCodeModel ViewParameter { get; }

    public static IEnumerable<TriggerTransitionCodeModel> FromTriggerTransition(TaskCodeInfo containingTask, ITriggerTransition triggerTransition) {

        foreach (var signalTrigger in triggerTransition.Triggers.OfType<ISignalTriggerSymbol>()) {

            var triggerCodeInfo = SignalTriggerCodeInfo.FromSignalTrigger(signalTrigger, containingTask);

            yield return new TriggerTransitionCodeModel(
                containingTask : containingTask,
                triggerCodeInfo: triggerCodeInfo,
                reachableCalls : triggerTransition.GetReachableCalls().ToImmutableList());
        }
    }

}