#region Using Directives

using System;
using System.Collections.Immutable;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen {
    class TransitionCodeModel : CodeModel {

        public TransitionCodeModel(ImmutableList<CallCodeModel> targetNodes) {
            Calls = targetNodes ?? throw new ArgumentNullException(nameof(targetNodes));
        }

        public ImmutableList<CallCodeModel> Calls { get; set; }
    }
}