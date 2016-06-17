using System;

namespace Pharmatechnik.Nav.Language.CodeGen {

    public sealed class SignalTriggerCodeGenInfo {

        public SignalTriggerCodeGenInfo(ISignalTriggerSymbol signalTriggerSymbol) {

            if (signalTriggerSymbol == null) {
                throw new ArgumentNullException(nameof(signalTriggerSymbol));
            }

            var task = signalTriggerSymbol.Transition.TaskDefinition;

            TaskCodeGenInfo        = new TaskCodeGenInfo(task);
            TriggerLogicMethodName = $"{signalTriggerSymbol.Name}Logic";
        }

        public TaskCodeGenInfo TaskCodeGenInfo { get; }
        public string TriggerLogicMethodName { get; }        
    }
}