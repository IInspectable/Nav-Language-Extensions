#region Using Directives

using System;
using System.Collections.Immutable;

using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen {

    class BeginWfsCodeModel: CodeModel {

        readonly TaskCodeModel _taskCodeModel;
        readonly ITaskDefinitionSymbol _taskDefinition;

        public BeginWfsCodeModel(ITaskDefinitionSymbol taskDefinition) {
            _taskDefinition = taskDefinition ?? throw new ArgumentNullException(nameof(taskDefinition));
            _taskCodeModel  = new TaskCodeModel(taskDefinition);
        }
        
        [NotNull]
        public string TaskName {
            get { return _taskDefinition.Name??string.Empty; }
        }

        [NotNull]
        public ITaskDefinitionSymbol TaskDefinition {
            get { return _taskDefinition; }
        }

        [NotNull]
        public string Namespace {
            get { return _taskCodeModel.WflNamespace; }
        }

        [NotNull]
        public ImmutableList<string> Namespaces {
            get { return GetCodeUsingNamespaces(_taskDefinition.CodeGenerationUnit); }
        }
    }
}
