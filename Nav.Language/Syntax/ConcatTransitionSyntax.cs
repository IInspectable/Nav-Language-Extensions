using System;

using JetBrains.Annotations;

using Pharmatechnik.Nav.Language.Text;

namespace Pharmatechnik.Nav.Language;

[Serializable]
[SampleSyntax("Node o-^ Target")]
public partial class ConcatTransitionSyntax: SyntaxNode {

    internal ConcatTransitionSyntax(TextExtent extent,
                                    EdgeSyntax edgeSyntax,
                                    TargetNodeSyntax targetNode): base(extent) {

        AddChildNode(Edge       = edgeSyntax);
        AddChildNode(TargetNode = targetNode);
    }

    [CanBeNull]
    public EdgeSyntax Edge { get; }

    [CanBeNull]
    public TargetNodeSyntax TargetNode { get; }

}