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

            AddChildNode(_codeNamespaceDeclaration      = codeNamespaceDeclaration);
            AddChildNode(_codeNotImplementedDeclaration = codeNotImplementedDeclaration);
            AddChildNode(_codeResultDeclaration         = codeResultDeclaration);
            AddChildNodes(_connectionPointNodes         = connectionPointNodeDeclarations);
        }

        public SyntaxToken TaskrefKeyword => ChildTokens().FirstOrMissing(SyntaxTokenType.TaskrefKeyword);
        public SyntaxToken Identifier     => ChildTokens().FirstOrMissing(SyntaxTokenType.Identifier);
        public SyntaxToken OpenBrace      => ChildTokens().FirstOrMissing(SyntaxTokenType.OpenBrace);
        public SyntaxToken CloseBrace     => ChildTokens().FirstOrMissing(SyntaxTokenType.CloseBrace);

        [CanBeNull]
        public CodeNamespaceDeclarationSyntax CodeNamespaceDeclaration => _codeNamespaceDeclaration;

        [CanBeNull]
        public CodeNotImplementedDeclarationSyntax CodeNotImplementedDeclaration => _codeNotImplementedDeclaration;

        [CanBeNull]
        public CodeResultDeclarationSyntax CodeResultDeclaration => _codeResultDeclaration;

        [NotNull]
        public IReadOnlyList<ConnectionPointNodeSyntax> ConnectionPoints => _connectionPointNodes;

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

        private protected override bool PromiseNoDescendantNodeOfSameType => true;
    }
}