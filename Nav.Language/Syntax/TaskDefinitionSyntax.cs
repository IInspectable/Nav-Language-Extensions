using System;
using JetBrains.Annotations;

using Pharmatechnik.Nav.Language.Text;

namespace Pharmatechnik.Nav.Language {
    [Serializable]
    [SampleSyntax("task Task { };")]
    public partial class TaskDefinitionSyntax : MemberDeclarationSyntax {

        readonly CodeDeclarationSyntax           _codeDeclaration;
        readonly CodeBaseDeclarationSyntax       _codeBaseDeclaration;
        readonly CodeGenerateToDeclarationSyntax _codeGenerateToDeclaration;
        readonly CodeParamsDeclarationSyntax     _codeParamsDeclaration;
        readonly CodeResultDeclarationSyntax     _codeResultDeclaration;
        readonly NodeDeclarationBlockSyntax      _nodeDeclarationBlock;
        readonly TransitionDefinitionBlockSyntax _transitionDefinitionBlock;

        internal TaskDefinitionSyntax(TextExtent extent, 
                                      CodeDeclarationSyntax codeDeclaration, 
                                      CodeBaseDeclarationSyntax codeBaseDeclaration,
                                      CodeGenerateToDeclarationSyntax codeGenerateToDeclaration, 
                                      CodeParamsDeclarationSyntax codeParamsDeclaration,
                                      CodeResultDeclarationSyntax codeResultDeclaration, 
                                      NodeDeclarationBlockSyntax nodeDeclarationBlock,
                                      TransitionDefinitionBlockSyntax transitionDefinitionBlock)
                : base(extent) {

            AddChildNode(_codeDeclaration           = codeDeclaration);
            AddChildNode(_codeBaseDeclaration       = codeBaseDeclaration);
            AddChildNode(_codeGenerateToDeclaration = codeGenerateToDeclaration);
            AddChildNode(_codeParamsDeclaration     = codeParamsDeclaration);
            AddChildNode(_codeResultDeclaration     = codeResultDeclaration);
            AddChildNode(_nodeDeclarationBlock      = nodeDeclarationBlock);
            AddChildNode(_transitionDefinitionBlock = transitionDefinitionBlock);
        }

        public SyntaxToken TaskKeyword => ChildTokens().FirstOrMissing(SyntaxTokenType.TaskKeyword);
        public SyntaxToken Identifier  => ChildTokens().FirstOrMissing(SyntaxTokenType.Identifier);
        public SyntaxToken OpenBrace   => ChildTokens().FirstOrMissing(SyntaxTokenType.OpenBrace);
        public SyntaxToken CloseBrace  => ChildTokens().FirstOrMissing(SyntaxTokenType.CloseBrace);

        [CanBeNull]
        public CodeDeclarationSyntax CodeDeclaration => _codeDeclaration;

        [CanBeNull]
        public CodeBaseDeclarationSyntax CodeBaseDeclaration => _codeBaseDeclaration;

        [CanBeNull]
        public CodeGenerateToDeclarationSyntax CodeGenerateToDeclaration => _codeGenerateToDeclaration;

        [CanBeNull]
        public CodeParamsDeclarationSyntax CodeParamsDeclaration => _codeParamsDeclaration;

        [CanBeNull]
        public CodeResultDeclarationSyntax CodeResultDeclaration => _codeResultDeclaration;

        [CanBeNull]
        public NodeDeclarationBlockSyntax NodeDeclarationBlock => _nodeDeclarationBlock;

        [CanBeNull]
        public TransitionDefinitionBlockSyntax TransitionDefinitionBlock => _transitionDefinitionBlock;

        private protected override bool PromiseNoDescendantNodeOfSameType => true;
    }
}