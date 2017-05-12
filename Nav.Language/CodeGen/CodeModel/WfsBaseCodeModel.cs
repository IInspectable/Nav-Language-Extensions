#region Using Directives

using System;
using System.Collections.Generic;
using System.Collections.Immutable;

using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen {

    sealed class WfsBaseCodeModel : CodeModel {

        WfsBaseCodeModel(ImmutableList<string> usingNamespaces, string iwflNamespace, string taskName) {
            UsingNamespaces   = usingNamespaces   ?? throw new ArgumentNullException();
            Namespace         = iwflNamespace     ?? throw new ArgumentNullException();
            TaskName          = taskName          ?? throw new ArgumentNullException();
        }

        [NotNull]
        public ImmutableList<string> UsingNamespaces { get; }
        [NotNull]
        public string Namespace { get; }
        [NotNull]
        public string TaskName { get; }

        public static WfsBaseCodeModel FromTaskDefinition(ITaskDefinitionSymbol taskDefinition) {

            if (taskDefinition == null) {
                throw new ArgumentNullException(nameof(taskDefinition));
            }

            var taskCodeModel = TaskCodeModel.FromTaskDefinition(taskDefinition);

            // UsingNamespaces
            var namespaces = new List<string>();
            namespaces.Add(taskCodeModel.IwflNamespace);
            namespaces.AddRange(taskDefinition.CodeGenerationUnit.GetCodeUsingNamespaces());
  
            return new WfsBaseCodeModel(
                usingNamespaces  : namespaces.ToImmutableList(), 
                iwflNamespace    : taskCodeModel.WflNamespace, 
                taskName         : taskDefinition.Name ?? string.Empty);
        }
    }
}