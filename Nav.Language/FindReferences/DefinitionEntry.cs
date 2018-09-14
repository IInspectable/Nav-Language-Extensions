#region Using Directives

using System;
using System.Collections.Immutable;

using Pharmatechnik.Nav.Language.Text;

#endregion

namespace Pharmatechnik.Nav.Language.FindReferences {

    public class DefinitionEntry {

        public DefinitionEntry(ISymbol symbol, ImmutableArray<ClassifiedText> displayParts) {
            Symbol       = symbol ?? throw new ArgumentNullException(nameof(symbol));
            DisplayParts = displayParts;
        }

        public ISymbol                        Symbol       { get; }
        public ImmutableArray<ClassifiedText> DisplayParts { get; }
        public string                         FullText     => DisplayParts.JoinText();

    }

}