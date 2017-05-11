using System;

namespace Pharmatechnik.Nav.Language.CodeGen {

    public sealed class TaskCodeModel: CodeModel {

        public TaskCodeModel(ITaskDefinitionSymbol taskDefinitionSymbol) {

            if (taskDefinitionSymbol == null) {
                throw new ArgumentNullException(nameof(taskDefinitionSymbol));
            }

            var task = taskDefinitionSymbol;

            var name = task.Name;
            var baseNamespace = (task.Syntax.SyntaxTree.GetRoot() as CodeGenerationUnitSyntax)?.CodeNamespace?.Namespace?.ToString();

            WflNamespace    = $"{baseNamespace}.WFL";
            IwflNamespace   = $"{baseNamespace}.IWFL";
            WfsBaseTypeName = $"{name}WFSBase";
            WfsTypeName     = $"{name}WFS";
        }

        public string WflNamespace { get; }
        public string IwflNamespace { get; }
        public string WfsBaseTypeName { get; }
        public string WfsTypeName { get; }

        public string FullyQualifiedWfsName {
            get { return $"{WflNamespace}.{WfsTypeName}"; }
        }

        public string FullyQualifiedWfsBaseName {
            get { return $"{WflNamespace}.{WfsBaseTypeName}"; }
        }
    }
}