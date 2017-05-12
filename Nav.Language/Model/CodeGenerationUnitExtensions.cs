#region Using Directives

using System.Collections.Immutable;
using System.Linq;
using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language {
    public static class CodeGenerationUnitExtensions {

        [CanBeNull]
        public static ITaskDefinitionSymbol TryFindTaskDefinition(this CodeGenerationUnit codeGenerationUnit, string taskName) {
            return codeGenerationUnit?.TaskDefinitions.TryFindSymbol(taskName);
        }

        public static ImmutableList<string> GetCodeUsingNamespaces(this CodeGenerationUnit codeGenerationUnit) {

            if (codeGenerationUnit == null) {
                return ImmutableList<string>.Empty;
            }

            var namespaces = codeGenerationUnit.Syntax
                .CodeUsings
                .Select(cu => cu.Namespace?.Text)
                .Where(ns => ns != null)
                .Distinct()
                .OrderBy(ns => ns.Length);

            return namespaces.ToImmutableList();
        }
    }
}