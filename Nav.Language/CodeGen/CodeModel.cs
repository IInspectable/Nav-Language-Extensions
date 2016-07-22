#region Using Directives

using System.Linq;
using System.Collections.Immutable;
using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen {

    public abstract class CodeModel {

        protected ImmutableList<string> GetCodeUsingNamespaces(CodeGenerationUnit codeGenerationUnit) {

            if(codeGenerationUnit == null) {
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

        [NotNull]
        public string ToCamelcase(string s) {

            if(string.IsNullOrEmpty(s)) {
                return s ?? string.Empty;
            }

            return s.Substring(0, 1).ToLowerInvariant() + s.Substring(1);
        }

        [NotNull]
        public string ToPascalcase(string s) {

            if (string.IsNullOrEmpty(s)) {
                return s ?? string.Empty;
            }

            return s.Substring(0, 1).ToUpperInvariant() + s.Substring(1);
        }

        [NotNull]
        public string ToFieldName(string s) {
            return s?.StartsWith(FieldPräfix) == true ? ToCamelcase(s) : $"{FieldPräfix}{ToCamelcase(s)}";
        }

        [NotNull]
        public string ToClassName(string s) {
            return ToPascalcase(s);
        }

        [NotNull]
        public string FieldPräfix {
            get { return "_"; }
        }

        public bool IsValidIdentifier(string value) {
            return CSharp.IsValidIdentifier(value);
        }
    }
}