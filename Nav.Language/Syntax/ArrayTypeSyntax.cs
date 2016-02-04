using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Pharmatechnik.Nav.Language {
    [Serializable]
    [SampleSyntax("Type[]")]
    public partial class ArrayTypeSyntax: CodeTypeSyntax {

        readonly IReadOnlyList<ArrayRankSpecifierSyntax> _rankSpecifiers;
        readonly CodeTypeSyntax _type;

        internal ArrayTypeSyntax(TextExtent extent, CodeTypeSyntax type, List<ArrayRankSpecifierSyntax> rankSpecifiers) 
            : base(extent) {

            AddChildNode( _type           = type);
            AddChildNodes(_rankSpecifiers = rankSpecifiers);
        }

        [CanBeNull]
        public CodeTypeSyntax Type {
            get { return _type; }
        }

        public int Rank {
            get { return RankSpecifiers.Count; }
        }

        [NotNull]
        public IReadOnlyList<ArrayRankSpecifierSyntax> RankSpecifiers {
            get { return _rankSpecifiers; }
        }
    }
}