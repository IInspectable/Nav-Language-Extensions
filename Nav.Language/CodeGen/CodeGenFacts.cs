#region Using Directives

using System;
using System.Linq;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen {

    public static partial class CodeGenFacts {

        internal static string BuildQualifiedNameQualifiedName(params string[] identifier) {
            var parts = identifier.Where(part => !String.IsNullOrEmpty(part)).ToList();

            if(!parts.Any()) {
                return String.Empty;
            }
            if(parts.Count == 1) {
                return parts[0];
            }

            return parts.Skip(1).Aggregate(parts[0], (s, part) => String.Join(".", s, part));
        }
    }
}