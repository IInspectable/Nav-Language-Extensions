using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Pharmatechnik.Nav.Language {
    [Serializable]
    [SampleSyntax("[base StandardWFS<TSType> : IWFServiceBase, IBeginWFSType]")]
    public partial class CodeBaseDeclarationSyntax : CodeSyntax {

        readonly IReadOnlyList<CodeTypeSyntax> _baseTypes;

        internal CodeBaseDeclarationSyntax(TextExtent extent, IReadOnlyList<CodeTypeSyntax> baseTypes)
            : base(extent) {
            AddChildNodes(_baseTypes = baseTypes);
        }

        public SyntaxToken BaseKeyword {
            get { return ChildTokens().FirstOrMissing(SyntaxTokenType.BaseKeyword); }
        }

        [NotNull]
        public IReadOnlyList<CodeTypeSyntax> BaseTypes {
            get { return _baseTypes; }
        }
    }
}