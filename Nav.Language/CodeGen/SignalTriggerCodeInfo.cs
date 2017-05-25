#region Using Directives

using System;

using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen {

    public sealed class SignalTriggerCodeInfo {
        
        SignalTriggerCodeInfo(TaskCodeInfo taskCodeInfo, string triggerMethodName, string triggerLogicMethodName, string toClassName) {
            TaskCodeInfo           = taskCodeInfo           ?? throw new ArgumentNullException(nameof(taskCodeInfo));
            TriggerMethodName      = triggerMethodName      ?? throw new ArgumentNullException(nameof(triggerMethodName));
            TriggerLogicMethodName = triggerLogicMethodName ?? throw new ArgumentNullException(nameof(triggerLogicMethodName));
            TOClassName            = toClassName            ?? throw new ArgumentNullException(nameof(toClassName));
        }

        public static SignalTriggerCodeInfo FromSignalTrigger(ISignalTriggerSymbol signalTriggerSymbol) {
            return FromSignalTrigger(signalTriggerSymbol, null);
        }

        internal static SignalTriggerCodeInfo FromSignalTrigger(ISignalTriggerSymbol signalTriggerSymbol, TaskCodeInfo taskCodeInfo) {

            if (signalTriggerSymbol == null) {
                throw new ArgumentNullException(nameof(signalTriggerSymbol));
            }

            var task     = signalTriggerSymbol.Transition.ContainingTask;
            var viewName = signalTriggerSymbol.Transition.Source?.Declaration?.Name??String.Empty;

            return new SignalTriggerCodeInfo(
                taskCodeInfo          : taskCodeInfo ?? TaskCodeInfo.FromTaskDefinition(task),
                triggerMethodName     : $"{signalTriggerSymbol.Name}",
                triggerLogicMethodName: $"{signalTriggerSymbol.Name}{CodeGenFacts.LogicMethodSuffix}",
                toClassName           : $"{viewName.ToPascalcase()}{CodeGenFacts.ToClassNameSuffix}"
            );
        }

        [NotNull]
        public TaskCodeInfo TaskCodeInfo { get; }
        [NotNull]
        public string TriggerMethodName { get; }
        [NotNull]
        public string TriggerLogicMethodName { get; }
        [NotNull]
        // ReSharper disable once InconsistentNaming
        public string TOClassName { get; }
    }
}