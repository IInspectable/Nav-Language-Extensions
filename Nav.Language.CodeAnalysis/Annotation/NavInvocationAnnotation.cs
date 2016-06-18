using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pharmatechnik.Nav.Language.CodeAnalysis.Annotation {

    public class NavInvocationAnnotation: NavTaskAnnotation {

        public IdentifierNameSyntax Identifier { get;  set; }
    }

}