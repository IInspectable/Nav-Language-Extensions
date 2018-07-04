using System;
using System.Collections.Generic;

using JetBrains.Annotations;

using Pharmatechnik.Nav.Language.Text;

namespace Pharmatechnik.Nav.Language {

    [Serializable]
    [SampleSyntax("Type[]")]
    public partial class ArrayTypeSyntax: CodeTypeSyntax {

        internal ArrayTypeSyntax(TextExtent extent, CodeTypeSyntax type, IReadOnlyList<ArrayRankSpecifierSyntax> rankSpecifiers)
            : base(extent) {

            AddChildNode(Type            = type);
            AddChildNodes(RankSpecifiers = rankSpecifiers);
        }

        [CanBeNull]
        public CodeTypeSyntax Type { get; }

        public int Rank => RankSpecifiers.Count;

        [NotNull]
        public IReadOnlyList<ArrayRankSpecifierSyntax> RankSpecifiers { get; }

    }

}