#region Using Directives

using System.Collections.Generic;
using System.Linq;

#endregion

namespace Pharmatechnik.Nav.Language {

    public static class DiagnosticExtensions {

        public static bool AddUnique(this List<Diagnostic> source, Diagnostic diagnostic) {
            if (source.Any(d => d.Descriptor == diagnostic.Descriptor && d.Location == diagnostic.Location)) {
                return false;
            }
            source.Add(diagnostic);
            return true;
        }

        public static List<Diagnostic> ToUnique(this List<Diagnostic> source) {
            var diagnostics = new List<Diagnostic>();

            foreach (var diagnostic in source) {
                diagnostics.AddUnique(diagnostic);
            }

            return diagnostics;
        }

        public static bool HasErrors(this IEnumerable<Diagnostic> source) {   
            return source.Errors().Any();
        }

        public static IEnumerable<Diagnostic> Warnings(this IEnumerable<Diagnostic> source) {
            return source.Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Warning);
        }

        public static IEnumerable<Diagnostic> Errors(this IEnumerable<Diagnostic> source) {
            return source.Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error);
        }
    }
}