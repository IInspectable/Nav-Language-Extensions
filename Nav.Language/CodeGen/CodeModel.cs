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
        public string ToFieldName(string s) {
            return s?.StartsWith(FieldPräfix) == true ? s.ToCamelcase() : $"{FieldPräfix}{s.ToCamelcase()}";
        }

        [NotNull]
        public string ToClassName(string s) {
            return s.ToPascalcase();
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