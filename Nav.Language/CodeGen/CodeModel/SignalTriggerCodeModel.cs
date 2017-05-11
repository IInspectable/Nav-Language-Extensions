#region Using Directives

using System;

using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen {

    public sealed class SignalTriggerCodeModel: CodeModel {

        public SignalTriggerCodeModel(ISignalTriggerSymbol signalTriggerSymbol) {

            if (signalTriggerSymbol == null) {
                throw new ArgumentNullException(nameof(signalTriggerSymbol));
            }

            var task = signalTriggerSymbol.Transition.ContainingTask;

            TaskCodeModel        = TaskCodeModel.FromTaskDefinition(task);
            TriggerLogicMethodName = $"{signalTriggerSymbol.Name}Logic";
        }

        [NotNull]
        public TaskCodeModel TaskCodeModel { get; }

        [NotNull]
        public string TriggerLogicMethodName { get; }        
    }
}