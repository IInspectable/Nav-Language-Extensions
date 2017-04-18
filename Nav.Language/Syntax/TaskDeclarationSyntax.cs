#region Using Directives

using System;
using System.Collections.Generic;

using JetBrains.Annotations;
using System.Linq;

#endregion

namespace Pharmatechnik.Nav.Language {

    [Serializable]
    [SampleSyntax("taskref Task { };")]
    public partial class TaskDeclarationSyntax : MemberDeclarationSyntax {

        readonly CodeNamespaceDeclarationSyntax           _codeNamespaceDeclaration;
        readonly CodeNotImplementedDeclarationSyntax      _codeNotImplementedDeclaration;
        readonly CodeResultDeclarationSyntax              _codeResultDeclaration;
        readonly IReadOnlyList<ConnectionPointNodeSyntax> _connectionPointNodes;


        internal TaskDeclarationSyntax(TextExtent extent, 
                                       CodeNamespaceDeclarationSyntax      codeNamespaceDeclaration,
                                       CodeNotImplementedDeclarationSyntax codeNotImplementedDeclaration, 
                                       CodeResultDeclarationSyntax         codeResultDeclaration, 
                                       IReadOnlyList<ConnectionPointNodeSyntax> connectionPointNodeDeclarations) 
            : base(extent) {

            AddChildNode(_codeNamespaceDeclaration  = codeNamespaceDeclaration);
            AddChildNode(_codeNotImplementedDeclaration = codeNotImplementedDeclaration);
            AddChildNode(_codeResultDeclaration         = codeResultDeclaration);
            AddChildNodes(_connectionPointNodes     = connectionPointNodeDeclarations);
        }

        public SyntaxToken TaskrefKeyword {
            get { return ChildTokens().FirstOrMissing(SyntaxTokenType.TaskrefKeyword); }
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
        public CodeNamespaceDeclarationSyntax CodeNamespaceDeclaration {
            get { return _codeNamespaceDeclaration; }
        }

        [CanBeNull]
        public CodeNotImplementedDeclarationSyntax CodeNotImplementedDeclaration {
            get { return _codeNotImplementedDeclaration; }
        }

        [CanBeNull]
        public CodeResultDeclarationSyntax CodeResultDeclaration {
            get { return _codeResultDeclaration; }
        }

        [NotNull]
        public IReadOnlyList<ConnectionPointNodeSyntax> ConnectionPoints {
            get { return _connectionPointNodes; }
        }

        [NotNull]
        public IEnumerable<InitNodeDeclarationSyntax> InitNodes() {
            return ConnectionPoints.OfType<InitNodeDeclarationSyntax>();
        }

        [NotNull]
        public IEnumerable<ExitNodeDeclarationSyntax> ExitNodes() {
            return ConnectionPoints.OfType<ExitNodeDeclarationSyntax>();
        }

        [NotNull]
        public IEnumerable<EndNodeDeclarationSyntax> EndNodes() {
            return ConnectionPoints.OfType<EndNodeDeclarationSyntax>();
        }

        protected override bool PromiseNoDescendantNodeOfSameType {
            get { return true; }
        }
    }
}