#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;

using Pharmatechnik.Nav.Language.Text;

namespace Pharmatechnik.Nav.Language;

[Serializable]
[SampleSyntax("")]
public partial class CodeGenerationUnitSyntax: SyntaxNode {

    internal CodeGenerationUnitSyntax(
        TextExtent extent,
        CodeNamespaceDeclarationSyntax? codeNamespaceDeclaration,
        IReadOnlyList<CodeUsingDeclarationSyntax> codeUsingDeclarations,
        IReadOnlyList<MemberDeclarationSyntax> memberDeclarations
    )
        : base(extent) {

        AddChildNode(CodeNamespace = codeNamespaceDeclaration);
        AddChildNodes(CodeUsings   = codeUsingDeclarations);
        AddChildNodes(Members      = memberDeclarations);
    }

    public CodeNamespaceDeclarationSyntax? CodeNamespace { get; }

    public IReadOnlyList<CodeUsingDeclarationSyntax> CodeUsings { get; }

    public IReadOnlyList<MemberDeclarationSyntax> Members { get; }

    public IReadOnlyList<IncludeDirectiveSyntax> Includes => Members.OfType<IncludeDirectiveSyntax>().ToList();

    public IReadOnlyList<TaskDeclarationSyntax> TaskDeclarations => Members.OfType<TaskDeclarationSyntax>().ToList();

    public IReadOnlyList<TaskDefinitionSyntax> TaskDefinitions => Members.OfType<TaskDefinitionSyntax>().ToList();

}