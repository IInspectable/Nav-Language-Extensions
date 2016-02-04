using System;
using JetBrains.Annotations;
using Pharmatechnik.Nav.Language.Internal;

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

        public SyntaxToken TaskKeyword {
            get { return ChildTokens().FirstOrMissing(SyntaxTokenType.TaskKeyword); }
        }

        public SyntaxToken Identifier {
            get { return ChildTokens().FirstOrMissing(SyntaxTokenType.Identifier); }
        }

        [SuppressCodeSanityCheck("Der Name IdentifierAlias ist hier ausdrücklich gewollt.")]
        public SyntaxToken IdentifierAlias {
            get { return Identifier.NextToken(SyntaxTokenType.Identifier); }
        }

        [CanBeNull]
        public CodeDoNotInjectDeclarationSyntax CodeDoNotInjectDeclaration {
            get { return _codeDoNotInjectDeclaration; }
        }

        [CanBeNull]
        public CodeAbstractMethodDeclarationSyntax CodeAbstractMethodDeclaration {
            get { return _codeAbstractMethodDeclaration; }
        }   
    }
}