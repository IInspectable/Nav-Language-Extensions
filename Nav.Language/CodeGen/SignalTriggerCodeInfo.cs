#region Using Directives

using System;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen {

    public sealed class SignalTriggerCodeInfo {

        SignalTriggerCodeInfo(TaskCodeInfo taskCodeInfo, string triggerName, string viewNodeName) {

            Task         = taskCodeInfo ?? throw new ArgumentNullException(nameof(taskCodeInfo));
            TriggerName  = triggerName  ?? String.Empty;
            ViewNodeName = viewNodeName ?? String.Empty;           
        }

        public TaskCodeInfo Task     { get; }
        public string ViewNodeName { get; }
        public string TriggerName  { get; }
        public string TriggerMethodName      => $"{TriggerName}";
        public string TriggerLogicMethodName => $"{TriggerName}{CodeGenFacts.LogicMethodSuffix}";
        public string TOClassName            => $"{ViewNodeName.ToPascalcase()}{CodeGenFacts.ToClassNameSuffix}";
        
        public static SignalTriggerCodeInfo FromSignalTrigger(ISignalTriggerSymbol signalTriggerSymbol) {
            return FromSignalTrigger(signalTriggerSymbol, null);
        }

        internal static SignalTriggerCodeInfo FromSignalTrigger(ISignalTriggerSymbol signalTriggerSymbol, TaskCodeInfo taskCodeInfo) {

            if (signalTriggerSymbol == null) {
                throw new ArgumentNullException(nameof(signalTriggerSymbol));
            }

            var task         = signalTriggerSymbol.Transition.ContainingTask;
            var viewNodeName = signalTriggerSymbol.Transition.GuiNodeReference?.Declaration?.Name??String.Empty;
            var triggerName  = signalTriggerSymbol.Name;

            return new SignalTriggerCodeInfo(
                taskCodeInfo: taskCodeInfo ?? TaskCodeInfo.FromTaskDefinition(task),
                triggerName : triggerName,
                viewNodeName: viewNodeName
            );
        }       
    }
}