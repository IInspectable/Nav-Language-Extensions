#region Using Directives

using System;
using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

using Pharmatechnik.Nav.Language.Text;

#endregion

namespace Pharmatechnik.Nav.Language; 

[Serializable]
[SampleSyntax("")]
public partial class CodeGenerationUnitSyntax: SyntaxNode {

    internal CodeGenerationUnitSyntax(
        TextExtent extent,
        CodeNamespaceDeclarationSyntax codeNamespaceDeclaration,
        IReadOnlyList<CodeUsingDeclarationSyntax> codeUsingDeclarations,
        IReadOnlyList<MemberDeclarationSyntax> memberDeclarations
    )
        : base(extent) {

        AddChildNode(CodeNamespace = codeNamespaceDeclaration);
        AddChildNodes(CodeUsings   = codeUsingDeclarations);
        AddChildNodes(Members      = memberDeclarations);
    }

    [CanBeNull]
    public CodeNamespaceDeclarationSyntax CodeNamespace { get; }

    [NotNull]
    public IReadOnlyList<CodeUsingDeclarationSyntax> CodeUsings { get; }

    [NotNull]
    public IReadOnlyList<MemberDeclarationSyntax> Members { get; }

    [NotNull]
    public IReadOnlyList<IncludeDirectiveSyntax> Includes => Members.OfType<IncludeDirectiveSyntax>().ToList();

    [NotNull]
    public IReadOnlyList<TaskDeclarationSyntax> TaskDeclarations => Members.OfType<TaskDeclarationSyntax>().ToList();

    [NotNull]
    public IReadOnlyList<TaskDefinitionSyntax> TaskDefinitions => Members.OfType<TaskDefinitionSyntax>().ToList();

}