#region Using Directives

using System;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis.CSharp.Syntax;

#endregion

namespace Pharmatechnik.Nav.Language.CodeAnalysis.Annotation; 

public partial class NavExitAnnotation: NavMethodAnnotation {

    public NavExitAnnotation(NavTaskAnnotation taskAnnotation, 
                             MethodDeclarationSyntax methodDeclaration, 
                             string exitTaskName): base(taskAnnotation, methodDeclaration) {
        ExitTaskName = exitTaskName ??String.Empty;
    }

    [NotNull]
    public string ExitTaskName { get;}
}