#region Using Directives

using System;
using System.Collections.Immutable;

using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen {

    class BeginWfsCodeModel: CodeModel {

        readonly TaskCodeGenInfo _taskCodeGenInfo;
        readonly ITaskDefinitionSymbol _taskDefinition;

        public BeginWfsCodeModel(ITaskDefinitionSymbol taskDefinition) {

            if (taskDefinition == null) {
                throw new ArgumentNullException(nameof(taskDefinition));
            }

            _taskDefinition  = taskDefinition;
            _taskCodeGenInfo = new TaskCodeGenInfo(taskDefinition);
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
            get { return _taskCodeGenInfo.WflNamespace; }
        }

        [NotNull]
        public ImmutableList<string> Namespaces {
            get { return GetCodeUsingNamespaces(_taskDefinition.CodeGenerationUnit); }
        }
    }
}
