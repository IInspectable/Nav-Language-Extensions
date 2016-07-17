#region Using Directives

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
    }
}