#region Using Directives

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language {

    [Serializable]
    [SampleSyntax("")]
    public partial class CompilationUnitSyntax : SyntaxNode {

        readonly CodeNamespaceDeclarationSyntax            _codeNamespace;
        readonly IReadOnlyList<CodeUsingDeclarationSyntax> _codeUsings;
        readonly IReadOnlyList<MemberDeclarationSyntax>    _members;

        internal CompilationUnitSyntax(
                TextExtent extent,
                CodeNamespaceDeclarationSyntax            codeNamespaceDeclaration,
                IReadOnlyList<CodeUsingDeclarationSyntax> codeUsingDeclarations,
                IReadOnlyList<MemberDeclarationSyntax>    memberDeclarations
                ) 
            : base(extent) {
            _members = memberDeclarations;

            AddChildNode( _codeNamespace = codeNamespaceDeclaration);
            AddChildNodes(_codeUsings    = codeUsingDeclarations   );
            AddChildNodes(_members       = memberDeclarations      );
        }

        [CanBeNull]
        public CodeNamespaceDeclarationSyntax CodeNamespace {
            get { return _codeNamespace; }
        }

        [NotNull]
        public IReadOnlyList<CodeUsingDeclarationSyntax> CodeUsings {
            get { return _codeUsings; }
        }

        [NotNull]
        public IReadOnlyList<MemberDeclarationSyntax> Members {
            get { return _members; }
        }

        [NotNull]
        public IReadOnlyList<IncludeDirectiveSyntax> Includes {
            get { return Members.OfType<IncludeDirectiveSyntax>().ToList(); }
        }

        [NotNull]
        public IReadOnlyList<TaskDeclarationSyntax> TaskDeclarations {
            get { return Members.OfType<TaskDeclarationSyntax>().ToList(); }
        }

        [NotNull]
        public IReadOnlyList<TaskDefinitionSyntax> TaskDefinitions {
            get { return Members.OfType<TaskDefinitionSyntax>().ToList(); }
        }        
    }
}
