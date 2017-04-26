﻿#region Using Directives

using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language {

    public interface IEdgeModeSymbol: ISymbol {
        EdgeMode EdgeMode { get; }
        [NotNull]
        ITransition Transition { get; }
    }
}