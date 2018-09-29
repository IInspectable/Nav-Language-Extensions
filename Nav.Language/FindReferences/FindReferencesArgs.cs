#region Using Directives

using System;

using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language.FindReferences {

    public class FindReferencesArgs {

        public FindReferencesArgs(ISymbol originatingSymbol, IFindReferencesContext context, NavSolution solution) {
            OriginatingSymbol = originatingSymbol ?? throw new ArgumentNullException(nameof(originatingSymbol));
            Context           = context           ?? throw new ArgumentNullException(nameof(context));
            Solution          = solution          ?? throw new ArgumentNullException(nameof(solution));

        }

        [NotNull]
        public ISymbol OriginatingSymbol { get; }

        [NotNull]
        public IFindReferencesContext Context { get; }

        [NotNull]
        public NavSolution Solution { get; }

    }

}