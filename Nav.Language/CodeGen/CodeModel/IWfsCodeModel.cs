#region Using Directives

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen {

    // ReSharper disable once InconsistentNaming
    public sealed class IWfsCodeModel: CodeModel {

        IWfsCodeModel(string syntaxFileName, TaskCodeModel taskCodeModel, ImmutableList<string> usingNamespaces, string taskName, string baseInterfaceName, ImmutableList<SignalTriggerCodeModel> signalTriggers) {
            SyntaxFileName    = syntaxFileName    ?? String.Empty;
            Task              = taskCodeModel     ?? throw new ArgumentNullException(nameof(taskCodeModel));
            UsingNamespaces   = usingNamespaces   ?? throw new ArgumentNullException(nameof(usingNamespaces));
            TaskName          = taskName          ?? throw new ArgumentNullException(nameof(taskName));
            BaseInterfaceName = baseInterfaceName ?? throw new ArgumentNullException(nameof(baseInterfaceName));
            SignalTriggers    = signalTriggers    ?? throw new ArgumentNullException(nameof(signalTriggers));
        }

        [NotNull]
        public string SyntaxFileName { get; }
        [NotNull]
        public TaskCodeModel Task { get; }
        [NotNull]
        public ImmutableList<string> UsingNamespaces { get; }
        [NotNull]
        public string Namespace => Task.IwflNamespace;
        [NotNull]
        public string TaskName { get; }
        [NotNull]
        public string BaseInterfaceName { get; }
        [NotNull]
        public ImmutableList<SignalTriggerCodeModel> SignalTriggers { get; }

        public static IWfsCodeModel FromTaskDefinition(ITaskDefinitionSymbol taskDefinition, PathProvider pathProvider) {

            if (taskDefinition == null) {
                throw new ArgumentNullException(nameof(taskDefinition));
            }
            if (pathProvider == null) {
                throw new ArgumentNullException(nameof(pathProvider));
            }

            var taskCodeModel = TaskCodeModel.FromTaskDefinition(taskDefinition);

            // UsingNamespaces
            var namespaces = new List<string>();
            namespaces.Add(taskCodeModel.IwflNamespace);
            namespaces.Add(CodeGenFacts.NavigationEngineIwflNamespace);
            namespaces.AddRange(taskDefinition.CodeGenerationUnit.GetCodeUsingNamespaces());

            // Signal Trigger
            var signalTriggers = new List<SignalTriggerCodeModel>();
            foreach (var trigger in taskDefinition.Transitions.SelectMany(t => t.Triggers).OfType<ISignalTriggerSymbol>()) {
                signalTriggers.Add(SignalTriggerCodeModel.FromSignalTrigger(trigger, taskCodeModel));
            }

            var syntaxFileName = pathProvider.GetRelativePath(pathProvider.IWfsFileName, pathProvider.SyntaxFileName);

            return new IWfsCodeModel(
                syntaxFileName   : syntaxFileName,
                taskCodeModel    : taskCodeModel,
                usingNamespaces  : namespaces.ToSortedNamespaces(), 
                taskName         : taskDefinition.Name ?? string.Empty,
                baseInterfaceName: taskDefinition.Syntax.CodeBaseDeclaration?.IwfsBaseType?.ToString() ?? CodeGenFacts.DefaultIwfsBaseType,
                signalTriggers   : signalTriggers.OrderBy(st=> st.TriggerMethodName.Length).ThenBy(st => st.TriggerMethodName).ToImmutableList());
        }
    }
}