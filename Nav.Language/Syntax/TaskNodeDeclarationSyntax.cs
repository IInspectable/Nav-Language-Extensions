using System;
using JetBrains.Annotations;
using Pharmatechnik.Nav.Language.Internal;
using Pharmatechnik.Nav.Language.Text;

namespace Pharmatechnik.Nav.Language {

    [Serializable]
    [SampleSyntax("task Identifier Alias [donotinject] [abstractmethod];")]
    public partial class TaskNodeDeclarationSyntax : NodeDeclarationSyntax {

        readonly CodeDoNotInjectDeclarationSyntax _codeDoNotInjectDeclaration;
        readonly CodeAbstractMethodDeclarationSyntax _codeAbstractMethodDeclaration;

        internal TaskNodeDeclarationSyntax(TextExtent extent,
                            CodeDoNotInjectDeclarationSyntax codeDoNotInjectDeclaration, 
                            CodeAbstractMethodDeclarationSyntax codeAbstractMethodDeclaration)
             : base(extent) {

            AddChildNode(_codeDoNotInjectDeclaration = codeDoNotInjectDeclaration);
            AddChildNode(_codeAbstractMethodDeclaration = codeAbstractMethodDeclaration);
        }

        public SyntaxToken TaskKeyword => ChildTokens().FirstOrMissing(SyntaxTokenType.TaskKeyword);

        public SyntaxToken Identifier => ChildTokens().FirstOrMissing(SyntaxTokenType.Identifier);

        [SuppressCodeSanityCheck("Der Name IdentifierAlias ist hier ausdr�cklich gewollt.")]
        public SyntaxToken IdentifierAlias => Identifier.NextToken(SyntaxTokenType.Identifier);

        [CanBeNull]
        public CodeDoNotInjectDeclarationSyntax CodeDoNotInjectDeclaration => _codeDoNotInjectDeclaration;

        [CanBeNull]
        public CodeAbstractMethodDeclarationSyntax CodeAbstractMethodDeclaration => _codeAbstractMethodDeclaration;
    }
}