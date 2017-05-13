#region Using Directives

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen {

    // ReSharper disable once InconsistentNaming
    sealed class IWfsCodeModel: CodeModel {

        IWfsCodeModel(ImmutableList<string> usingNamespaces, string iwflNamespace, string taskName, string baseInterfaceName, ImmutableList<SignalTriggerCodeModel> signalTriggers) {
            UsingNamespaces   = usingNamespaces   ?? throw new ArgumentNullException();
            Namespace         = iwflNamespace     ?? throw new ArgumentNullException();
            TaskName          = taskName          ?? throw new ArgumentNullException();
            BaseInterfaceName = baseInterfaceName ?? throw new ArgumentNullException();
            SignalTriggers    = signalTriggers    ?? throw new ArgumentNullException();
        }

        [NotNull]
        public ImmutableList<string> UsingNamespaces { get; }
        [NotNull]
        public string Namespace { get; }
        [NotNull]
        public string TaskName { get; }
        [NotNull]
        public string BaseInterfaceName { get; }
        [NotNull]
        public ImmutableList<SignalTriggerCodeModel> SignalTriggers { get; }

        public static IWfsCodeModel FromTaskDefinition(ITaskDefinitionSymbol taskDefinition) {

            if (taskDefinition == null) {
                throw new ArgumentNullException(nameof(taskDefinition));
            }

            var taskCodeModel = TaskCodeModel.FromTaskDefinition(taskDefinition);

            // UsingNamespaces
            var namespaces = new List<string>();
            namespaces.Add(taskCodeModel.IwflNamespace);
            namespaces.AddRange(taskDefinition.CodeGenerationUnit.GetCodeUsingNamespaces());

            // Signal Trigger
            var signalTriggers = new List<SignalTriggerCodeModel>();
            foreach (var trigger in taskDefinition.Transitions.SelectMany(t => t.Triggers).OfType<ISignalTriggerSymbol>()) {
                signalTriggers.Add(SignalTriggerCodeModel.FromSignalTrigger(trigger, taskCodeModel));
            }

            return new IWfsCodeModel(
                usingNamespaces  : namespaces.ToImmutableList(), 
                iwflNamespace    : taskCodeModel.IwflNamespace, 
                taskName         : taskDefinition.Name ?? string.Empty,
                baseInterfaceName: taskDefinition.Syntax.CodeBaseDeclaration?.IwfsBaseType?.ToString() ?? DefaultIwfsBaseType,
                signalTriggers   : signalTriggers.OrderBy(st=> st.TriggerMethodName).ToImmutableList());
        }
    }
}