#region Using Directives

using System;
using System.Collections.Immutable;
using System.IO;

using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language.FindReferences {

    public class FindReferencesArgs {

        public FindReferencesArgs(ISymbol symbol, IFindReferencesContext context, DirectoryInfo solutionRoot, ImmutableArray<FileInfo> solutionFiles) {
            Symbol        = symbol  ?? throw new ArgumentNullException(nameof(symbol));
            Context       = context ?? throw new ArgumentNullException(nameof(context));
            SolutionRoot  = solutionRoot;
            SolutionFiles = solutionFiles;

        }

        [NotNull]
        public ISymbol Symbol { get; }

        // TODO SearchDirectory sollte mit
        [CanBeNull]
        public DirectoryInfo SolutionRoot { get; }

        // TODO Eher so etwas wie einen NavWorkspace /NavSolution mitgeben
        public ImmutableArray<FileInfo> SolutionFiles { get; }

        [NotNull]
        public IFindReferencesContext Context { get; }

    }

}