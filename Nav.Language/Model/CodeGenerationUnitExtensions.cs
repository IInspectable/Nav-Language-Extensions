#region Using Directives

using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language {
    public static class CodeGenerationUnitExtensions {

        [CanBeNull]
        public static ITaskDefinitionSymbol TryFindTaskDefinition(this CodeGenerationUnit codeGenerationUnit, string taskName) {
            return codeGenerationUnit?.TaskDefinitions.TryFindSymbol(taskName);
        }
    }
}