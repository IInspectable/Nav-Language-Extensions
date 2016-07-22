﻿#region Using Directives

using System.Linq;
using System.Collections.Immutable;

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

        public string ToCamelcase(string s) {

            if(string.IsNullOrEmpty(s)) {
                return s ?? string.Empty;
            }

            return s.Substring(0, 1).ToLowerInvariant() + s.Substring(1);
        }

        public string ToPascalcase(string s) {

            if (string.IsNullOrEmpty(s)) {
                return s ?? string.Empty;
            }

            return s.Substring(0, 1).ToUpperInvariant() + s.Substring(1);
        }

        public string ToFieldName(string s) {
            return s?.StartsWith(FieldPräfix) == true ? ToCamelcase(s) : $"{FieldPräfix}{ToCamelcase(s)}";
        }

        public string ToClassName(string s) {
            return ToPascalcase(s);
        }

        public string FieldPräfix {
            get { return "_"; }
        }

        public bool IsValidIdentifier(string value) {
            return CSharp.IsValidIdentifier(value);
        }
    }
}