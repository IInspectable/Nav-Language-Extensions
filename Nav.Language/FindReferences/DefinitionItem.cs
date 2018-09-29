#region Using Directives

using System;
using System.Collections.Immutable;

using Pharmatechnik.Nav.Language.Text;

#endregion

namespace Pharmatechnik.Nav.Language.FindReferences {

    public class DefinitionItem {

        public DefinitionItem(string solutionRoot,
                              ISymbol symbol,
                              ImmutableArray<ClassifiedText> textParts,
                              bool expandedByDefault = true,
                              string sortKey = null) {

            SolutionRoot      = solutionRoot;
            Symbol            = symbol ?? throw new ArgumentNullException(nameof(symbol));
            TextParts         = textParts;
            ExpandedByDefault = expandedByDefault;
            SortKey           = sortKey ?? "";
        }

        public string                         SolutionRoot      { get; }
        public ISymbol                        Symbol            { get; }
        public Location                       Location          => Symbol.Location;
        public ImmutableArray<ClassifiedText> TextParts         { get; }
        public bool                           ExpandedByDefault { get; } // TODO In etwas allgemeineres umbenennen?
        public string                         SortKey           { get; }

        public string Text     => TextParts.JoinText();
        public string SortText => SortKey + Text;

    }

}