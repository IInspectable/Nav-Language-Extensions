#region Using Directives

using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen {
    class TriggerTransitionCodeModel : TransitionCodeModel {

        public TriggerTransitionCodeModel(ParameterCodeModel viewParameter, ImmutableList<Call> reachableCalls, string viewName, string triggerName)
            : base(reachableCalls) {
            TriggerName = triggerName;
            ViewName    = viewName ?? String.Empty;
            ViewParameter = viewParameter;

        }

        public string ViewName { get; }
        public string ViewNamePascalcase => ViewName.ToPascalcase();
        public string TriggerName { get; }
        public string TriggerNamePascalcase => TriggerName.ToPascalcase();
        public ParameterCodeModel ViewParameter { get; } 

        public static IEnumerable<TriggerTransitionCodeModel> FromTriggerTransition(ITransition triggerTransition) {

            var guiNode = triggerTransition?.Source?.Declaration as IGuiNodeSymbol;
            if(guiNode == null) {
                throw new ArgumentException("Trigger transition expected");
            }

            foreach(var signalTrigger in triggerTransition.Triggers.OfType<ISignalTriggerSymbol>()) {
                yield return new TriggerTransitionCodeModel(
                    reachableCalls: triggerTransition.GetReachableCalls().ToImmutableList(),
                    viewName      : guiNode.Name,
                    triggerName   : signalTrigger.Name,
                    viewParameter : new ParameterCodeModel(guiNode.Name.ToPascalcase()+CodeGenFacts.ToClassNameSuffix, "to"));
            }
        }
    }
}