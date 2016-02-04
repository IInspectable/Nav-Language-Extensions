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
        public IdentifierSourceNodeSyntax SourceNode {
            get { return _sourceNode; }
        }

        public SyntaxToken Colon {
            get { return ChildTokens().FirstOrMissing(SyntaxTokenType.Colon); }
        }

        [SuppressCodeSanityCheck("Der Name ExitIdentifier ist hier ausdrücklich gewollt.")]
        public SyntaxToken ExitIdentifier {
            get { return ChildTokens().FirstOrMissing(SyntaxTokenType.Identifier); }
        }
        
        [CanBeNull]
        public EdgeSyntax Edge {
            get { return _edge; }
        }

        [CanBeNull]
        public TargetNodeSyntax TargetNode {
            get { return _targetNode; }
        }

        [CanBeNull]
        public ConditionClauseSyntax ConditionClause {
            get { return _conditionClause; }
        }

        [CanBeNull]
        public DoClauseSyntax DoClause {
            get { return _doClause; }
        }

        public SyntaxToken Semicolon {
            get { return ChildTokens().FirstOrMissing(SyntaxTokenType.Semicolon); }
        }        
    }
}