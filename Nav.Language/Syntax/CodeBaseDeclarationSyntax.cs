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

        // TODO WfsBaseType dürfte eigentlich nie null sein?
        [CanBeNull]
        public CodeTypeSyntax WfsBaseType {
            get {
                if (_baseTypes.Count == 0) {
                    return null;
                }
                return _baseTypes[0];
            }
        }

        [CanBeNull]
        public CodeTypeSyntax IwfsBaseType {
            get {
                if (_baseTypes.Count < 2) {
                    return null;
                }
                return _baseTypes[1];
            }
        }

        [CanBeNull]
        // ReSharper disable once InconsistentNaming
        public CodeTypeSyntax IBeginWfsBaseType {
            get {
                if (_baseTypes.Count < 3) {
                    return null;
                }
                return _baseTypes[2];
            }
        }

        [NotNull]
        public IReadOnlyList<CodeTypeSyntax> BaseTypes {
            get { return _baseTypes; }
        }
    }
}