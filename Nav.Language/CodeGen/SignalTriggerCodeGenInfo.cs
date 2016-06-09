using System;

namespace Pharmatechnik.Nav.Language.CodeGen {

    public sealed class SignalTriggerCodeGenInfo {

        public SignalTriggerCodeGenInfo(ISignalTriggerSymbol signalTriggerSymbol) {

            if (signalTriggerSymbol == null) {
                throw new ArgumentNullException(nameof(signalTriggerSymbol));
            }

            var task = signalTriggerSymbol.Transition.TaskDefinition;

            var name = task.Name;
            var baseNamespace = (task.Syntax.SyntaxTree.GetRoot() as CodeGenerationUnitSyntax)?.CodeNamespace?.Namespace?.ToString();

            WflNamespace      = $"{baseNamespace}.WFL";
            WfsTypeName       = $"{name}WFS";
            TriggerMethodName = $"{signalTriggerSymbol.Name}Logic";
        }


        public string WflNamespace { get; }
        public string WfsTypeName { get; }
        public string TriggerMethodName { get; }

        public string WfsFullyQualifiedName {
            get { return $"{WflNamespace}.{WfsTypeName}"; }
        }
    }
}
