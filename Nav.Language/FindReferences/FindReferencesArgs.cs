#region Using Directives

using System;

using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language.FindReferences {

    public class FindReferencesArgs {

        public FindReferencesArgs(ISymbol symbol, IFindReferencesContext context, NavSolution solution) {
            Symbol   = symbol   ?? throw new ArgumentNullException(nameof(symbol));
            Context  = context  ?? throw new ArgumentNullException(nameof(context));
            Solution = solution ?? throw new ArgumentNullException(nameof(solution));

        }

        [NotNull]
        public ISymbol Symbol { get; }

        [NotNull]
        public IFindReferencesContext Context { get; }

        [NotNull]
        public NavSolution Solution { get; }

    }

}