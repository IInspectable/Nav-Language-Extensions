using System;

using JetBrains.Annotations;

using Pharmatechnik.Nav.Language.Text;

namespace Pharmatechnik.Nav.Language; 

[Serializable]
[SampleSyntax("task Task { };")]
public partial class TaskDefinitionSyntax: MemberDeclarationSyntax {

    internal TaskDefinitionSyntax(TextExtent extent,
                                  CodeDeclarationSyntax codeDeclaration,
                                  CodeBaseDeclarationSyntax codeBaseDeclaration,
                                  CodeGenerateToDeclarationSyntax codeGenerateToDeclaration,
                                  CodeParamsDeclarationSyntax codeParamsDeclaration,
                                  CodeResultDeclarationSyntax codeResultDeclaration,
                                  NodeDeclarationBlockSyntax nodeDeclarationBlock,
                                  TransitionDefinitionBlockSyntax transitionDefinitionBlock)
        : base(extent) {

        AddChildNode(CodeDeclaration           = codeDeclaration);
        AddChildNode(CodeBaseDeclaration       = codeBaseDeclaration);
        AddChildNode(CodeGenerateToDeclaration = codeGenerateToDeclaration);
        AddChildNode(CodeParamsDeclaration     = codeParamsDeclaration);
        AddChildNode(CodeResultDeclaration     = codeResultDeclaration);
        AddChildNode(NodeDeclarationBlock      = nodeDeclarationBlock);
        AddChildNode(TransitionDefinitionBlock = transitionDefinitionBlock);
    }

    public SyntaxToken TaskKeyword => ChildTokens().FirstOrMissing(SyntaxTokenType.TaskKeyword);
    public SyntaxToken Identifier  => ChildTokens().FirstOrMissing(SyntaxTokenType.Identifier);
    public SyntaxToken OpenBrace   => ChildTokens().FirstOrMissing(SyntaxTokenType.OpenBrace);
    public SyntaxToken CloseBrace  => ChildTokens().FirstOrMissing(SyntaxTokenType.CloseBrace);

    [CanBeNull]
    public CodeDeclarationSyntax CodeDeclaration { get; }

    [CanBeNull]
    public CodeBaseDeclarationSyntax CodeBaseDeclaration { get; }

    [CanBeNull]
    public CodeGenerateToDeclarationSyntax CodeGenerateToDeclaration { get; }

    [CanBeNull]
    public CodeParamsDeclarationSyntax CodeParamsDeclaration { get; }

    [CanBeNull]
    public CodeResultDeclarationSyntax CodeResultDeclaration { get; }

    [CanBeNull]
    public NodeDeclarationBlockSyntax NodeDeclarationBlock { get; }

    [CanBeNull]
    public TransitionDefinitionBlockSyntax TransitionDefinitionBlock { get; }

    private protected override bool PromiseNoDescendantNodeOfSameType => true;

}