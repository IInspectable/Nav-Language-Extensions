#region Using Directives

using System;

// ReSharper disable InconsistentNaming

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen {

    public sealed class TaskCodeInfo {
        
        TaskCodeInfo(string taskName, string baseNamespace, string wfsBaseBaseClassName) {
            TaskName            = taskName             ?? String.Empty;
            BaseNamespace       = baseNamespace        ?? String.Empty;
            WfsBaseBaseTypeName = wfsBaseBaseClassName ?? String.Empty;
        }

        string BaseNamespace              { get; }
        public string TaskName            { get; }        
        public string WfsBaseBaseTypeName { get; }

        public string TaskNamePascalcase        => TaskName.ToPascalcase();
        public string WflNamespace              => BuildQualifiedName(BaseNamespace, CodeGenFacts.WflNamespaceSuffix);
        public string IwflNamespace             => BuildQualifiedName(BaseNamespace, CodeGenFacts.IwflNamespaceSuffix);        
        public string WfsBaseTypeName           => $"{TaskNamePascalcase}{CodeGenFacts.WfsBaseClassSuffix}";
        public string WfsTypeName               => $"{TaskNamePascalcase}{CodeGenFacts.WfsClassSuffix}";
        public string IWfsTypeName              => $"{CodeGenFacts.InterfacePrefix}{TaskNamePascalcase}{CodeGenFacts.WfsClassSuffix}";
        public string FullyQualifiedWfsName     => BuildQualifiedName(WflNamespace, WfsTypeName);
        public string FullyQualifiedWfsBaseName => BuildQualifiedName(WflNamespace, WfsBaseTypeName);

        public static TaskCodeInfo FromTaskDefinition(ITaskDefinitionSymbol taskDefinition) {

            if (taskDefinition == null) {
                throw new ArgumentNullException(nameof(taskDefinition));
            }

            var taskName = taskDefinition.Name;
            var baseNamespace = (taskDefinition.Syntax.SyntaxTree.GetRoot() as CodeGenerationUnitSyntax)?.CodeNamespace?.Namespace?.ToString() ?? String.Empty;
            var wfsBaseBaseClassName = taskDefinition.Syntax.CodeBaseDeclaration?.WfsBaseType?.ToString() ?? CodeGenFacts.DefaultWfsBaseClass;

            return new TaskCodeInfo(
                taskName            : taskName,
                baseNamespace       : baseNamespace,
                wfsBaseBaseClassName: wfsBaseBaseClassName);
        }

        static string BuildQualifiedName(params string[] identifier) {
            return CodeGenFacts.BuildQualifiedName(identifier);
        }
    }
}