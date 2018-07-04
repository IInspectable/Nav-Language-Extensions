#region Using Directives

using System;
using System.Collections.Generic;

using JetBrains.Annotations;

using System.Linq;

using Pharmatechnik.Nav.Language.Text;

#endregion

namespace Pharmatechnik.Nav.Language {

    [Serializable]
    [SampleSyntax("taskref Task { };")]
    public partial class TaskDeclarationSyntax: MemberDeclarationSyntax {

        internal TaskDeclarationSyntax(TextExtent extent,
                                       CodeNamespaceDeclarationSyntax codeNamespaceDeclaration,
                                       CodeNotImplementedDeclarationSyntax codeNotImplementedDeclaration,
                                       CodeResultDeclarationSyntax codeResultDeclaration,
                                       IReadOnlyList<ConnectionPointNodeSyntax> connectionPoints)
            : base(extent) {

            AddChildNode(CodeNamespaceDeclaration      = codeNamespaceDeclaration);
            AddChildNode(CodeNotImplementedDeclaration = codeNotImplementedDeclaration);
            AddChildNode(CodeResultDeclaration         = codeResultDeclaration);
            AddChildNodes(ConnectionPoints             = connectionPoints);
        }

        public SyntaxToken TaskrefKeyword => ChildTokens().FirstOrMissing(SyntaxTokenType.TaskrefKeyword);
        public SyntaxToken Identifier     => ChildTokens().FirstOrMissing(SyntaxTokenType.Identifier);
        public SyntaxToken OpenBrace      => ChildTokens().FirstOrMissing(SyntaxTokenType.OpenBrace);
        public SyntaxToken CloseBrace     => ChildTokens().FirstOrMissing(SyntaxTokenType.CloseBrace);

        [CanBeNull]
        public CodeNamespaceDeclarationSyntax CodeNamespaceDeclaration { get; }

        [CanBeNull]
        public CodeNotImplementedDeclarationSyntax CodeNotImplementedDeclaration { get; }

        [CanBeNull]
        public CodeResultDeclarationSyntax CodeResultDeclaration { get; }

        [NotNull]
        public IReadOnlyList<ConnectionPointNodeSyntax> ConnectionPoints { get; }

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