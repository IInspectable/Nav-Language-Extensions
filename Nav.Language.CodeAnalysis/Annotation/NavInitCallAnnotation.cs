#region Using Directives

using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis.CSharp.Syntax;

#endregion

namespace Pharmatechnik.Nav.Language.CodeAnalysis.Annotation {

    public partial class NavInitCallAnnotation: NavInvocationAnnotation {

        public NavInitCallAnnotation(NavTaskAnnotation taskAnnotation, 
                                        IdentifierNameSyntax identifier, 
                                        string beginItfFullyQualifiedName, 
                                        List<string> parameter)
            : base(taskAnnotation, identifier) {

            BeginItfFullyQualifiedName = beginItfFullyQualifiedName ?? String.Empty;
            Parameter                  = parameter                  ?? new List<string>();
        }

        [NotNull]
        public string BeginItfFullyQualifiedName { get;}

        [NotNull]
        public List<string> Parameter { get; }
    }
}