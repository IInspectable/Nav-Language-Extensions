#region Using Directives

using System;
using System.IO;

using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language.FindReferences {

    public class FindReferencesArgs {

        public FindReferencesArgs(ISymbol symbol, DirectoryInfo searchDirectory) {
            Symbol          = symbol ?? throw new ArgumentNullException(nameof(symbol));
            SearchDirectory = searchDirectory;

        }

        [NotNull]
        public ISymbol Symbol { get; }

        [CanBeNull]
        public DirectoryInfo SearchDirectory { get; }

    }

}