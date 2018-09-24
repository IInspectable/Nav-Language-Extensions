#region Using Directives

using System;
using System.Collections.Immutable;

using Pharmatechnik.Nav.Language.Text;

#endregion

namespace Pharmatechnik.Nav.Language.FindReferences {

    public class DefinitionItem {

        public DefinitionItem(string solutionRoot, ISymbol symbol, ImmutableArray<ClassifiedText> textParts) {
            SolutionRoot = solutionRoot;
            Symbol       = symbol ?? throw new ArgumentNullException(nameof(symbol));
            TextParts    = textParts;
        }

        public string                         SolutionRoot { get; }
        public ISymbol                        Symbol       { get; }
        public Location                       Location     => Symbol.Location;
        public ImmutableArray<ClassifiedText> TextParts    { get; }

        public string Text => TextParts.JoinText();

    }

}