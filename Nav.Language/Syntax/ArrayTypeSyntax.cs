using System;
using System.Collections.Generic;
using JetBrains.Annotations;

using Pharmatechnik.Nav.Language.Text;

namespace Pharmatechnik.Nav.Language {
    [Serializable]
    [SampleSyntax("Type[]")]
    public partial class ArrayTypeSyntax: CodeTypeSyntax {

        readonly IReadOnlyList<ArrayRankSpecifierSyntax> _rankSpecifiers;
        readonly CodeTypeSyntax _type;

        internal ArrayTypeSyntax(TextExtent extent, CodeTypeSyntax type, IReadOnlyList<ArrayRankSpecifierSyntax> rankSpecifiers) 
            : base(extent) {

            AddChildNode( _type           = type);
            AddChildNodes(_rankSpecifiers = rankSpecifiers);
        }

        [CanBeNull]
        public CodeTypeSyntax Type => _type;

        public int Rank => RankSpecifiers.Count;

        [NotNull]
        public IReadOnlyList<ArrayRankSpecifierSyntax> RankSpecifiers => _rankSpecifiers;
    }
}