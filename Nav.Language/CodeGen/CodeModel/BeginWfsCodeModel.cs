#region Using Directives

using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen {

    sealed class BeginWfsCodeModel : CodeModel {

        public BeginWfsCodeModel(ITaskDefinitionSymbol taskDefinition) {

            if (taskDefinition == null) {
                throw new ArgumentNullException(nameof(taskDefinition));
            }

            var taskCodeModel = new TaskCodeModel(taskDefinition);
            
            // UsingNamespaces
            var namespaces = new List<string>();
            namespaces.Add(taskCodeModel.IwflNamespace);
            namespaces.AddRange(GetCodeUsingNamespaces(taskDefinition.CodeGenerationUnit));
           
            // Inits
            var taskInits = new List<TaskInitCodeModel>();
            foreach (var initNode in taskDefinition.NodeDeclarations.OfType<IInitNodeSymbol>()) {
                var taskInit = TaskInitCodeModel.FromInitNode(initNode, taskCodeModel);
                taskInits.Add(taskInit);
            }
            
            UsingNamespaces        = namespaces.ToImmutableList();
            Namespace         = taskCodeModel.WflNamespace;
            TaskName          = taskDefinition.Name ?? string.Empty;
            BaseInterfaceName = "IBeginWFService";
            TaskInits         = taskInits.ToImmutableList();
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