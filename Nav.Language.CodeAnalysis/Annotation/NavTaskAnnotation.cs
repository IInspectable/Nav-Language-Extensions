#region Using Directives

using System;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis.CSharp.Syntax;

#endregion

namespace Pharmatechnik.Nav.Language.CodeAnalysis.Annotation {

    public partial class NavTaskAnnotation {

        public NavTaskAnnotation(ClassDeclarationSyntax classDeclarationSyntax, string taskName, string navFileName) {

            if (classDeclarationSyntax == null) {
                throw new ArgumentNullException(nameof(classDeclarationSyntax));
            }
           
            ClassDeclarationSyntax = classDeclarationSyntax;
            TaskName               = taskName    ??String.Empty;
            NavFileName            = navFileName ?? String.Empty;
        }

        protected NavTaskAnnotation(NavTaskAnnotation other) {

            if (other == null) {
                throw new ArgumentNullException(nameof(other));
            }

            ClassDeclarationSyntax = other.ClassDeclarationSyntax;
            TaskName               = other.TaskName;
            NavFileName            = other.NavFileName;
        }

        [NotNull]
        public ClassDeclarationSyntax ClassDeclarationSyntax { get; }

        [NotNull]
        public string TaskName { get;}

        [NotNull]
        public string NavFileName { get; }
    }
}