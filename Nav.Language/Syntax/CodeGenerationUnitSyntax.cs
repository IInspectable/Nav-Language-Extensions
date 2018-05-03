#region Using Directives

using System;
using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

using Pharmatechnik.Nav.Language.Text;

#endregion

namespace Pharmatechnik.Nav.Language {

    [Serializable]
    [SampleSyntax("")]
    public partial class CodeGenerationUnitSyntax: SyntaxNode {

        readonly CodeNamespaceDeclarationSyntax            _codeNamespace;
        readonly IReadOnlyList<CodeUsingDeclarationSyntax> _codeUsings;
        readonly IReadOnlyList<MemberDeclarationSyntax>    _members;

        internal CodeGenerationUnitSyntax(
            TextExtent extent,
            CodeNamespaceDeclarationSyntax codeNamespaceDeclaration,
            IReadOnlyList<CodeUsingDeclarationSyntax> codeUsingDeclarations,
            IReadOnlyList<MemberDeclarationSyntax> memberDeclarations
        )
            : base(extent) {

            _members = memberDeclarations;

            AddChildNode(_codeNamespace = codeNamespaceDeclaration);
            AddChildNodes(_codeUsings   = codeUsingDeclarations);
            AddChildNodes(_members      = memberDeclarations);
        }

        [CanBeNull]
        public CodeNamespaceDeclarationSyntax CodeNamespace => _codeNamespace;

        [NotNull]
        public IReadOnlyList<CodeUsingDeclarationSyntax> CodeUsings => _codeUsings;

        [NotNull]
        public IReadOnlyList<MemberDeclarationSyntax> Members => _members;

        [NotNull]
        public IReadOnlyList<IncludeDirectiveSyntax> Includes => Members.OfType<IncludeDirectiveSyntax>().ToList();

        [NotNull]
        public IReadOnlyList<TaskDeclarationSyntax> TaskDeclarations => Members.OfType<TaskDeclarationSyntax>().ToList();

        [NotNull]
        public IReadOnlyList<TaskDefinitionSyntax> TaskDefinitions => Members.OfType<TaskDefinitionSyntax>().ToList();

    }

}