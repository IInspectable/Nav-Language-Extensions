#region Using Directives

using System;
using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen {

    public sealed class TaskCodeModel: CodeModel {
        
        TaskCodeModel(string taskName, string wflNamespace, string iwflNamespace, string wfsBaseTypeName, string wfsTypeName) {
            TaskName        = taskName       ?? String.Empty;
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
                taskName       : taskName,
                wflNamespace   : $"{baseNamespace}.{CodeGenFacts.WflNamespaceSuffix}",
                iwflNamespace  : $"{baseNamespace}.{CodeGenFacts.IwflNamespaceSuffix}",
                wfsBaseTypeName: $"{taskName}{CodeGenFacts.WfsBaseClassSuffix}",
                wfsTypeName    : $"{taskName}{CodeGenFacts.WfsClassSuffix}");
        }

        [NotNull]
        public string TaskName { get; }
        [NotNull]
        public string WflNamespace { get; }
        [NotNull]
        public string IwflNamespace { get; }
        [NotNull]
        public string WfsBaseTypeName { get; }
        [NotNull]
        public string WfsTypeName { get; }
        [NotNull]
        public string FullyQualifiedWfsName     => $"{WflNamespace}.{WfsTypeName}";
        [NotNull]
        public string FullyQualifiedWfsBaseName => $"{WflNamespace}.{WfsBaseTypeName}";
    }
}