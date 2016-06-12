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

            WflNamespace           = $"{baseNamespace}.WFL";
            WfsBaseTypeName        = $"{name}WFSBase";
            WfsTypeName            = $"{name}WFS";
            TriggerLogicMethodName = $"{signalTriggerSymbol.Name}Logic";
        }

        public string WflNamespace { get; }
        public string WfsBaseTypeName { get; }
        public string WfsTypeName { get; }
        public string TriggerLogicMethodName { get; }

        public string FullyQualifiedWfsName {
            get { return $"{WflNamespace}.{WfsTypeName}"; }
        }

        public string FullyQualifiedWfsBaseName {
            get { return $"{WflNamespace}.{WfsBaseTypeName}"; }
        }
    }
}
