using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pharmatechnik.Nav.Language.CodeAnalysis.Annotation {

    public abstract class NavMethodAnnotation: NavTaskAnnotation {

        public MethodDeclarationSyntax MethodDeclarationSyntax {
            get; internal set;
        }
    }
}