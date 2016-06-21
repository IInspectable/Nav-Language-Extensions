#region Using Directives

using System;
using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen {

    public sealed class SignalTriggerCodeGenInfo {

        public SignalTriggerCodeGenInfo(ISignalTriggerSymbol signalTriggerSymbol) {

            if (signalTriggerSymbol == null) {
                throw new ArgumentNullException(nameof(signalTriggerSymbol));
            }

            var task = signalTriggerSymbol.Transition.ContainingTask;

            TaskCodeGenInfo        = new TaskCodeGenInfo(task);
            TriggerLogicMethodName = $"{signalTriggerSymbol.Name}Logic";
        }

        [NotNull]
        public TaskCodeGenInfo TaskCodeGenInfo { get; }

        [NotNull]
        public string TriggerLogicMethodName { get; }        
    }
}