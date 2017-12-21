using System;
using JetBrains.Annotations;

namespace Pharmatechnik.Nav.Language {
    [Serializable]
    [SampleSyntax("[params Type1 p1, Type2 p2]")]
    public partial class CodeParamsDeclarationSyntax : CodeSyntax {

        readonly ParameterListSyntax _parameterList;

        internal CodeParamsDeclarationSyntax(TextExtent extent, ParameterListSyntax parameterList)
            : base(extent) {
            AddChildNode(_parameterList = parameterList);
        }

        public SyntaxToken ParamsKeyword => ChildTokens().FirstOrMissing(SyntaxTokenType.ParamsKeyword);

        [CanBeNull]
        public ParameterListSyntax ParameterList => _parameterList;
    }
}