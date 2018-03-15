namespace Pharmatechnik.Nav.Language {

    public static partial class DiagnosticDescriptors {
       
        public static class Syntax {

            public const DiagnosticCategory Category = DiagnosticCategory.Syntax;
            public const DiagnosticSeverity Severity = DiagnosticSeverity.Error;
           
            /// <summary>
            /// Unexpected character '{0}'
            /// </summary>
            public static readonly DiagnosticDescriptor Nav0000UnexpectedCharacter = new DiagnosticDescriptor(
                id             : DiagnosticId.Nav0000,
                messageFormat  : "Unexpected character '{0}'",
                category       : Category,
                defaultSeverity: Severity
                );
        }
    }
}