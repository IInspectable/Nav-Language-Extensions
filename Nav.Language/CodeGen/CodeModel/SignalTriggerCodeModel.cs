#region Using Directives

using System;

using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen {

    public sealed class SignalTriggerCodeModel: CodeModel {
        
        SignalTriggerCodeModel(TaskCodeModel taskCodeModel, string triggerMethodName, string triggerLogicMethodName, string toClassName) {
            TaskCodeModel          = taskCodeModel          ?? throw new ArgumentNullException(nameof(taskCodeModel));
            TriggerMethodName      = triggerMethodName      ?? throw new ArgumentNullException(nameof(triggerMethodName));
            TriggerLogicMethodName = triggerLogicMethodName ?? throw new ArgumentNullException(nameof(triggerLogicMethodName));
            TOClassName            = toClassName            ?? throw new ArgumentNullException(nameof(triggerLogicMethodName));
        }

        public static SignalTriggerCodeModel FromSignalTrigger(ISignalTriggerSymbol signalTriggerSymbol) {
            return FromSignalTrigger(signalTriggerSymbol, null);
        }

        internal static SignalTriggerCodeModel FromSignalTrigger(ISignalTriggerSymbol signalTriggerSymbol, TaskCodeModel taskCodeModel) {

            if (signalTriggerSymbol == null) {
                throw new ArgumentNullException(nameof(signalTriggerSymbol));
            }

            var task     = signalTriggerSymbol.Transition.ContainingTask;
            var viewName = signalTriggerSymbol.Transition.Source?.Declaration?.Name??String.Empty;

            return new SignalTriggerCodeModel(
                taskCodeModel         : taskCodeModel ?? TaskCodeModel.FromTaskDefinition(task),
                triggerMethodName     : $"{signalTriggerSymbol.Name}",
                triggerLogicMethodName: $"{signalTriggerSymbol.Name}{CodeGenFacts.LogicMethodSuffix}",
                toClassName           : $"{viewName.ToPascalcase()}{CodeGenFacts.ToClassNameSuffix}"
            );
        }

        [NotNull]
        public TaskCodeModel TaskCodeModel { get; }
        [NotNull]
        public string TriggerMethodName { get; }
        [NotNull]
        public string TriggerLogicMethodName { get; }
        [NotNull]
        public string TOClassName { get; }
    }
}