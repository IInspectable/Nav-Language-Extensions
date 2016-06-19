using System.Collections.Generic;

namespace Pharmatechnik.Nav.Language.CodeAnalysis.Annotation {

    public partial class NavInitCallAnnotation: NavInvocationAnnotation {
        
        public string BeginItfFullyQualifiedName { get;  set; }

        public List<string> Parameter { get; set; }
    }
}