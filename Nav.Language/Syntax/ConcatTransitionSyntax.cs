#nullable enable

using System;

using Pharmatechnik.Nav.Language.Text;

namespace Pharmatechnik.Nav.Language;

[Serializable]
[SampleSyntax("o-^ Target")]
public partial class ConcatTransitionSyntax: SyntaxNode {

    internal ConcatTransitionSyntax(TextExtent extent,
                                    ConcatEdgeSyntax? edgeSyntax,
                                    TargetNodeSyntax? targetNode): base(extent) {

        AddChildNode(Edge       = edgeSyntax);
        AddChildNode(TargetNode = targetNode);
    }

    public ConcatEdgeSyntax? Edge { get; }

    public TargetNodeSyntax? TargetNode { get; }

}