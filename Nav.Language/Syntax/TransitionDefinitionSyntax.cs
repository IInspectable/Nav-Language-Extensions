using System;
using JetBrains.Annotations;

namespace Pharmatechnik.Nav.Language {

    [Serializable]
    [SampleSyntax("Node --> Target on Trigger if Condition do Instruction;")]
    public partial class TransitionDefinitionSyntax : SyntaxNode {

        readonly SourceNodeSyntax      _sourceNode;
        readonly EdgeSyntax            _edge;
        readonly TargetNodeSyntax      _targetNode;
        readonly TriggerSyntax         _trigger;
        readonly ConditionClauseSyntax _conditionClause;
        readonly DoClauseSyntax        _doClause;

        internal TransitionDefinitionSyntax(TextExtent extent,
                                            SourceNodeSyntax sourceNode,
                                            EdgeSyntax edgeSyntax,
                                            TargetNodeSyntax targetNode,
                                            TriggerSyntax trigger,
                                            ConditionClauseSyntax conditionClause,
                                            DoClauseSyntax doClause) : base(extent) {

            AddChildNode(_sourceNode      = sourceNode);
            AddChildNode(_edge            = edgeSyntax);
            AddChildNode(_targetNode      = targetNode);
            AddChildNode(_trigger         = trigger);
            AddChildNode(_conditionClause = conditionClause);
            AddChildNode(_doClause        = doClause);
        }
        
        [CanBeNull]
        public SourceNodeSyntax SourceNode => _sourceNode;

        [CanBeNull]
        public EdgeSyntax Edge => _edge;

        [CanBeNull]
        public TargetNodeSyntax TargetNode => _targetNode;

        [CanBeNull]
        public TriggerSyntax Trigger => _trigger;

        [CanBeNull]
        public ConditionClauseSyntax ConditionClause => _conditionClause;

        [CanBeNull]
        public DoClauseSyntax DoClause => _doClause;

        public SyntaxToken Semicolon => ChildTokens().FirstOrMissing(SyntaxTokenType.Semicolon);
    }
}