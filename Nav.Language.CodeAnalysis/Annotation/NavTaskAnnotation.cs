using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pharmatechnik.Nav.Language.CodeAnalysis.Annotation {

    public partial class NavTaskAnnotation {

        public ClassDeclarationSyntax ClassDeclarationSyntax { get; internal set; }
        public string TaskName { get; internal set; }
        public string NavFileName { get; internal set; }
    }
}