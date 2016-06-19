using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pharmatechnik.Nav.Language.CodeAnalysis.Annotation {

    public abstract class NavInvocationAnnotation: NavTaskAnnotation {

        public IdentifierNameSyntax Identifier { get;  set; }
    }
}