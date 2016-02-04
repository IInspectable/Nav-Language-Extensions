namespace Pharmatechnik.Nav.Language {

    public static partial class DiagnosticDescriptors {

        // Unbenutzer Code wird etwas gesondert behandelt, da weder Error noch Warning.
        // Im Editor wird der Code z.B. etwas abgeduckellt dargestellt. Deshalb bekommt
        // diese Art von Diagnostic eine eigene Kategorie.
        public static class DeadCode {

            public const DiagnosticCategory Category = DiagnosticCategory.DeadCode;
            public const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;

            /// <summary>
            /// The include directive for the file '{0}' appeared previously in this file\r\nInclude Directive is unnecessary
            /// </summary>
            public static readonly DiagnosticDescriptor Nav1001IncludeDirectiveForFile0AppearedPreviously = new DiagnosticDescriptor(
                id             : DiagnosticId.Nav1001,
                messageFormat  : "The include directive for the file '{0}' appeared previously in this file",
                category       : Category,
                defaultSeverity: Severity
                );

            /// <summary>
            /// The using directive for '{0}' appeared previously in this file.\r\nUsing Directive is unnecessary
            /// </summary>
            public static readonly DiagnosticDescriptor Nav1002UsingDirective0AppearedPreviously = new DiagnosticDescriptor(
                id             : DiagnosticId.Nav1002,
                messageFormat  : "The using directive for '{0}' appeared previously in this file and can be safely removed",
                category       : Category,
                defaultSeverity: Severity
                );

            /// <summary>
            /// Taskref directive is not required by the code and can be safely removed
            /// </summary>
            public static readonly DiagnosticDescriptor Nav1003IncludeNotRequired = new DiagnosticDescriptor(
                id             : DiagnosticId.Nav1003,
                messageFormat  : "Taskref directive is not required by the code and can be safely removed",
                category       : Category,
                defaultSeverity: Severity
                );

            /// <summary>
            /// Node '{0}' is not required by the code and can be safely removed
            /// </summary>
            public static readonly DiagnosticDescriptor Nav1004Node0NotRequired = new DiagnosticDescriptor(
                id             : DiagnosticId.Nav1004,
                messageFormat  : "Node '{0}' is not required by the code and can be safely removed",
                category       : Category,
                defaultSeverity: Severity
                );

            /// <summary>
            /// Taskref '{0}' is not required by the code and can be safely removed
            /// </summary>
            public static readonly DiagnosticDescriptor Nav1005TaskDeclaration0NotRequired = new DiagnosticDescriptor(
                id             : DiagnosticId.Nav1005,
                messageFormat  : "Taskref '{0}' is not required by the code and can be safely removed",
                category       : Category,
                defaultSeverity: Severity
                );

            /// <summary>
            /// The self referencing taskref directive is not required by the code and can be safely removed
            /// </summary>
            public static readonly DiagnosticDescriptor Nav1006SelfReferencingIncludeNotRequired = new DiagnosticDescriptor(
                id: DiagnosticId.Nav1006,
                messageFormat  : "The self referencing taskref directive is not required by the code and can be safely removed",
                category       : Category,
                defaultSeverity: Severity
                );
        }
    }
}