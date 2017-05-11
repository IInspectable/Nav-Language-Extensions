#region Using Directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Immutable;

using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen {

    // TODO Keine Referenzen auf Semantic Model halten
    class BeginWfsCodeModel: CodeModel {

        readonly TaskCodeModel _taskCodeModel;
        readonly ITaskDefinitionSymbol _taskDefinition;

        public BeginWfsCodeModel(ITaskDefinitionSymbol taskDefinition) {

            _taskDefinition = taskDefinition ?? throw new ArgumentNullException(nameof(taskDefinition));
            _taskCodeModel  = new TaskCodeModel(taskDefinition);

            var taskBegins = new List<TaskInitCodeModel>();
            foreach (var init in _taskDefinition.NodeDeclarations.OfType<IInitNodeSymbol>()) {
                var taskBegin = TaskInitCodeModel.FromInitNode(init, _taskCodeModel);
                taskBegins.Add(taskBegin);
            }
            TaskBegins = taskBegins.ToImmutableList();
        }
        

        [NotNull]
        public string TaskName {
            get { return _taskDefinition.Name??string.Empty; }
        }

        [NotNull]
        public string BaseInterfaceName => "IBeginWFService";

        [NotNull]
        public string Namespace {
            get { return _taskCodeModel.WflNamespace; }
        }

        [NotNull]
        public string IwflNamespace {
            get { return _taskCodeModel.IwflNamespace; }
        }

        [NotNull]
        public ImmutableList<string> Namespaces {
            get { return GetCodeUsingNamespaces(_taskDefinition.CodeGenerationUnit); }
        }

        [NotNull]
        public ImmutableList<TaskInitCodeModel> TaskBegins { get; }       
    }
}
