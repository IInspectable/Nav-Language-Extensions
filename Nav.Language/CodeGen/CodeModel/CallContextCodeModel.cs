#nullable enable

using System.Collections.Immutable;

namespace Pharmatechnik.Nav.Language.CodeGen;

sealed class ContinuationCodeModel: CodeModel {

    public required CallCodeModel         Call         { get; init; }
    public required BeginWrapperCodeModel BeginWrapper { get; init; }

}

sealed class CallContextCodeModel: CodeModel {

    public          string                               ClassName      => Parent.GetCallContextClassName();
    public required TaskCodeInfo                         ContainingTask { get; init; }
    public required TransitionCodeModel                  Parent         { get; init; }
    public required string                               ViewName       { get; init; }
    public required ImmutableList<ContinuationCodeModel> Continuations  { get; init; }

}