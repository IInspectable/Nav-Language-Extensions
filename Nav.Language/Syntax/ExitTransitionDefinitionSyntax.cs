using System;
using JetBrains.Annotations;
using Pharmatechnik.Nav.Language.Internal;

namespace Pharmatechnik.Nav.Language {

    [Serializable]
    [SampleSyntax("SourceNode:ExitIdentifier --> TargetNode if Condition do Instruction;")]
    public partial class ExitTransitionDefinitionSyntax : SyntaxNode {

        readonly IdentifierSourceNodeSyntax _sourceNode;
        readonly EdgeSyntax                 _edge;
        readonly TargetNodeSyntax           _targetNode;
        readonly ConditionClauseSyntax      _conditionClause;
        readonly DoClauseSyntax             _doClause;

        internal ExitTransitionDefinitionSyntax(TextExtent extent,
                                                IdentifierSourceNodeSyntax sourceNode,
                                                EdgeSyntax edge,
                                                TargetNodeSyntax targetNode,
                                                ConditionClauseSyntax conditionClause, 
                                                DoClauseSyntax doClause) : base(extent) {
            
            AddChildNode(_sourceNode      = sourceNode);
            AddChildNode(_edge            = edge);
            AddChildNode(_targetNode      = targetNode);
            AddChildNode(_conditionClause = conditionClause);
            AddChildNode(_doClause        = doClause);
        }

        [CanBeNull]
        public IdentifierSourceNodeSyntax SourceNode => _sourceNode;

        public SyntaxToken Colon => ChildTokens().FirstOrMissing(SyntaxTokenType.Colon);

        [SuppressCodeSanityCheck("Der Name ExitIdentifier ist hier ausdrücklich gewollt.")]
        public SyntaxToken ExitIdentifier => ChildTokens().FirstOrMissing(SyntaxTokenType.Identifier);

        [CanBeNull]
        public EdgeSyntax Edge => _edge;

        [CanBeNull]
        public TargetNodeSyntax TargetNode => _targetNode;

        [CanBeNull]
        public ConditionClauseSyntax ConditionClause => _conditionClause;

        [CanBeNull]
        public DoClauseSyntax DoClause => _doClause;

        public SyntaxToken Semicolon => ChildTokens().FirstOrMissing(SyntaxTokenType.Semicolon);
    }
}