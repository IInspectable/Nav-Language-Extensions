#region Using Directives

using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen {

    // ReSharper disable once InconsistentNaming
    sealed class IBeginWfsCodeModel : CodeModel {

        IBeginWfsCodeModel(ImmutableList<string> usingNamespaces, string wflNamespace, string taskName, string baseInterfaceName, ImmutableList<TaskInitCodeModel> taskInits) {
            UsingNamespaces   = usingNamespaces   ?? throw new ArgumentNullException(nameof(usingNamespaces));
            Namespace         = wflNamespace      ?? throw new ArgumentNullException(nameof(usingNamespaces));
            TaskName          = taskName          ?? throw new ArgumentNullException(nameof(usingNamespaces));
            BaseInterfaceName = baseInterfaceName ?? throw new ArgumentNullException(nameof(usingNamespaces));
            TaskInits         = taskInits         ?? throw new ArgumentNullException(nameof(usingNamespaces));
        }

        public static IBeginWfsCodeModel FromTaskDefinition(ITaskDefinitionSymbol taskDefinition) {

            if (taskDefinition == null) {
                throw new ArgumentNullException(nameof(taskDefinition));
            }

            var taskCodeModel = TaskCodeModel.FromTaskDefinition(taskDefinition);

            // UsingNamespaces
            var namespaces = new List<string>();
            namespaces.Add(taskCodeModel.IwflNamespace);
            namespaces.AddRange(taskDefinition.CodeGenerationUnit.GetCodeUsingNamespaces());

            // Inits
            var taskInits = new List<TaskInitCodeModel>();
            foreach (var initNode in taskDefinition.NodeDeclarations.OfType<IInitNodeSymbol>()) {
                var taskInit = TaskInitCodeModel.FromInitNode(initNode, taskCodeModel);
                taskInits.Add(taskInit);
            }

            return new IBeginWfsCodeModel (
             usingNamespaces   : namespaces.ToImmutableList(),
             wflNamespace      : taskCodeModel.WflNamespace,
             taskName          : taskDefinition.Name ?? string.Empty,
             baseInterfaceName : taskDefinition.Syntax.CodeBaseDeclaration?.IBeginWfsBaseType?.ToString()?? DefaultIBeginWfsBaseType,
             taskInits         : taskInits.ToImmutableList());
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
        public ImmutableList<TaskInitCodeModel> TaskInits { get; }       
    }
}