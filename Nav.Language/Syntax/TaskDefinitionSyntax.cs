using System;
using JetBrains.Annotations;

namespace Pharmatechnik.Nav.Language {
    [Serializable]
    [SampleSyntax("task Task { };")]
    public partial class TaskDefinitionSyntax : MemberDeclarationSyntax {

        readonly CodeDeclarationSyntax           _codeDeclaration;
        readonly CodeBaseDeclarationSyntax           _codeBaseDeclaration;
        readonly CodeGenerateToDeclarationSyntax     _codeGenerateToDeclaration;
        readonly CodeParamsDeclarationSyntax         _codeParamsDeclaration;
        readonly CodeResultDeclarationSyntax         _codeResultDeclaration;
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
            AddChildNode(_codeBaseDeclaration           = codeBaseDeclaration);
            AddChildNode(_codeGenerateToDeclaration     = codeGenerateToDeclaration);
            AddChildNode(_codeParamsDeclaration         = codeParamsDeclaration);
            AddChildNode(_codeResultDeclaration         = codeResultDeclaration);
            AddChildNode(_nodeDeclarationBlock      = nodeDeclarationBlock);
            AddChildNode(_transitionDefinitionBlock = transitionDefinitionBlock);
        }

        public SyntaxToken TaskKeyword {
            get { return ChildTokens().FirstOrMissing(SyntaxTokenType.TaskKeyword); }
        }

        public SyntaxToken Identifier {
            get { return ChildTokens().FirstOrMissing(SyntaxTokenType.Identifier); }
        }

        public SyntaxToken OpenBrace {
            get { return ChildTokens().FirstOrMissing(SyntaxTokenType.OpenBrace); }
        }

        public SyntaxToken CloseBrace {
            get { return ChildTokens().FirstOrMissing(SyntaxTokenType.CloseBrace); }
        }

        [CanBeNull]
        public CodeDeclarationSyntax CodeDeclaration {
            get { return _codeDeclaration; }
        }

        [CanBeNull]
        public CodeBaseDeclarationSyntax CodeBaseDeclaration {
            get { return _codeBaseDeclaration; }
        }

        [CanBeNull]
        public CodeGenerateToDeclarationSyntax CodeGenerateToDeclaration {
            get { return _codeGenerateToDeclaration; }
        }

        [CanBeNull]
        public CodeParamsDeclarationSyntax CodeParamsDeclaration {
            get { return _codeParamsDeclaration; }
        }

        [CanBeNull]
        public CodeResultDeclarationSyntax CodeResultDeclaration {
            get { return _codeResultDeclaration; }
        }

        [CanBeNull]
        public NodeDeclarationBlockSyntax NodeDeclarationBlock {
            get { return _nodeDeclarationBlock; }
        }

        [CanBeNull]
        public TransitionDefinitionBlockSyntax TransitionDefinitionBlock {
            get { return _transitionDefinitionBlock; }
        }
    }
}