#region Using Directives

using System;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen {

    public sealed class TaskCodeModel: CodeModel {
        
        TaskCodeModel(string wflNamespace, string iwflNamespace, string wfsBaseTypeName, string wfsTypeName) {
            WflNamespace    = wflNamespace    ?? String.Empty;
            IwflNamespace   = iwflNamespace   ?? String.Empty;
            WfsBaseTypeName = wfsBaseTypeName ?? String.Empty;
            WfsTypeName     = wfsTypeName     ?? String.Empty;
        }

        public static TaskCodeModel FromTaskDefinition(ITaskDefinitionSymbol taskDefinition) {

            if (taskDefinition == null) {
                throw new ArgumentNullException(nameof(taskDefinition));
            }

            var taskName      = taskDefinition.Name;
            var baseNamespace = (taskDefinition.Syntax.SyntaxTree.GetRoot() as CodeGenerationUnitSyntax)?.CodeNamespace?.Namespace?.ToString() ?? String.Empty;

            return new TaskCodeModel(
                wflNamespace   : $"{baseNamespace}.WFL",
                iwflNamespace  : $"{baseNamespace}.IWFL",
                wfsBaseTypeName: $"{taskName}WFSBase",
                wfsTypeName    : $"{taskName}WFS");
        }

        public string WflNamespace { get; }
        public string IwflNamespace { get; }
        public string WfsBaseTypeName { get; }
        public string WfsTypeName { get; }
        public string FullyQualifiedWfsName     => $"{WflNamespace}.{WfsTypeName}";
        public string FullyQualifiedWfsBaseName => $"{WflNamespace}.{WfsBaseTypeName}";
    }
}